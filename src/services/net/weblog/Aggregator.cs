using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Nohros.Data.Json;
using ZMQ;
using Nohros.Ruby.Protocol;
using Nohros.Concurrent;
using Nohros.Resources;

namespace Nohros.Ruby.Logging
{
  /// <summary>
  /// A class that aggregates log messages send from anywhere (possible from a
  /// service) publish and persist them to a database.
  /// </summary>
  public class Aggregator : AbstractRubyService
  {
    const string kClassName = "Nohros.Ruby.Logging.Aggregator";
    const string kJsonFeedName = "json";
    readonly Context context_;

    readonly IDictionary<string, string> facts_;

    readonly IRubyLogger logger_ = RubyLogger.ForCurrentProcess;
    readonly Mailbox<LogMessage> mailbox_;
    readonly Socket publisher_;
    readonly IAggregatorSettings settings_;
    readonly ManualResetEvent start_stop_event_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="Aggregator"/> class
    /// by using the specified ZMQ sockets.
    /// </summary>
    public Aggregator(Context context, IAggregatorSettings settings) {
      settings_ = settings;
      mailbox_ = new Mailbox<LogMessage>(ProcessLogMessage);
      context_ = context;
      start_stop_event_ = new ManualResetEvent(false);
      publisher_ = context.Socket(SocketType.PUB);
      context_ = context;

      facts_ = new Dictionary<string, string>();
      InitFacts();
    }
    #endregion

    public override void Start(IRubyServiceHost service_host) {
      publisher_.Bind("tcp://*:" + settings_.PublisherPort);

      // Self host means that we are not using the ruby node service, the
      // service is beign hosted ony by the ruby .NET infrastructure, which
      // does not provide a mechanism to receive messages from outside. We
      // need to create a channel to receive message for your own.
      if (settings_.SelfHost) {
        StartSelfHosting();
      } else {
        // Wait stop to be called.
        start_stop_event_.WaitOne();
        start_stop_event_.Close();
      }

      publisher_.Dispose();
      context_.Dispose();
    }

    public void StartSelfHosting() {
      try {
        Socket receiver = context_.Socket(SocketType.DEALER);
        receiver.Bind(Transport.TCP, "127.0.0.1", (uint) settings_.SelfHostPort);
        while (!start_stop_event_.WaitOne(0)) {
          RubyMessagePacket packet =
            RubyMessagePacket.ParseFrom(receiver.Recv());
          OnMessage(packet.Message);
        }
      } catch (ZMQ.Exception zmqe) {
        if (zmqe.Errno == (int) ERRNOS.ETERM) {
          // We can do nothing with a closed context. Stop the service.
          if (logger_.IsWarnEnabled) {
            logger_.Warn("Context was terminated while the service was running.");
          }
          Stop(null);
        }
      } catch (System.Exception exception) {
        LogMessage log = GetInternalLogMessage(exception);
        Publish(log);
        Store(log);
      }
    }

    public override void Stop(IRubyMessage message) {
      start_stop_event_.Set();
    }

    /// <inheritdoc/>
    public override void OnMessage(IRubyMessage message) {
      switch (message.Type) {
        case (int) LoggingMessageType.kLogMessage:
          OnLogMessage(message);
          break;
      }
    }

    void ProcessLogMessage(LogMessage log) {
      try {
        LogMessage.Types.Metadata metadata = log.Medatata;
        if (metadata.Publish) {
          Publish(log);
        }

        if (metadata.Store) {
          Store(log);
        }
      } catch (System.Exception exception) {
        logger_.Error(
          string.Format(StringResources.Log_MethodThrowsException, kClassName,
            "Start"), exception);
      }
    }

    void OnLogMessage(IRubyMessage message) {
      // A try/catch block is used here to capture parsing exceptions.
      try {
        LogMessage log = LogMessage.ParseFrom(message.Message);
        mailbox_.Send(log);
      } catch (System.Exception exception) {
        LogMessage log = GetInternalLogMessage(exception);
        Publish(log);
        Store(log);
      }
    }

    /// <summary>
    /// Gets a <see cref="LogMessage"/> object that represents the given
    /// exception.
    /// </summary>
    /// <returns>
    /// A <see cref="LogMessage"/> object that represents
    /// <paramref name="exception"/>.
    /// </returns>
    LogMessage GetInternalLogMessage(System.Exception exception) {
      return new LogMessage.Builder()
        .SetApplication(Strings.kApplicationName)
        .SetLevel("ERROR")
        .SetReason(exception.Message)
        .SetTimeStamp(TimeUnitHelper.ToUnixTime(DateTime.Now))
        .SetUser(Environment.UserName)
        .AddCategorization(new KeyValuePair.Builder()
          .SetKey("backtrace")
          .SetValue(exception.StackTrace))
        .Build();
    }

    /// <summary>
    /// Publish the message to any conneted subscriber.
    /// </summary>
    /// <param name="message">
    /// The message to publish.
    /// </param>
    void Publish(LogMessage message) {
      try {
        // publish to the JSON feed.
        publisher_.SendMore(
          kJsonFeedName + ":" + message.Application, Encoding.UTF8);
        publisher_.Send(GetJson(message), Encoding.UTF8);

        if (logger_.IsDebugEnabled) {
          logger_.Debug("Published a message to the JSON feed");
        }

        // TODO(neylor.silva): Publish the message to the zeromq feed.
      } catch (System.Exception exception) {
        Store(GetInternalLogMessage(exception));
      }
    }

    string GetJson(LogMessage message) {
      return new JsonStringBuilder()
        .WriteBeginObject()
        .WriteMember("application", message.Application)
        .WriteMember("level", message.Level)
        .WriteMember("reason", message.Reason)
        .WriteMember("timestamp", message.TimeStamp.ToString())
        .WriteEndObject()
        .ToString();
    }

    /// <summary>
    /// Persist the messaget o a database.
    /// </summary>
    /// <param name="message">
    /// The message to be persisted.
    /// </param>
    void Store(LogMessage message) {
      try {
        settings_.AggregatorDataProvider.Store(message);
      } catch (System.Exception exception) {
        // This is the last place where an internal raised exception could
        // reach. So, at this point we need to log the message using our
        // internal logger.
        logger_.Error(
          string.Format(StringResources.Log_MethodThrowsException, kClassName,
            "Store"), exception);
      }
    }

    void InitFacts() {
      facts_.Add("service-name", "nohros.ruby.log");
      facts_.Add("can-process-message", "cfa950a0ca0611e19b230800200c9a66");
    }

    /// <inheritdoc/>
    public override IDictionary<string, string> Facts {
      get { return facts_; }
    }
  }
}
