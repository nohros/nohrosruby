using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
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

    readonly IDictionary<string, string> facts_;

    readonly IRubyLogger logger_ = RubyLogger.ForCurrentProcess;
    readonly Mailbox<LogMessage> mailbox_;
    readonly Context context_;
    Socket publisher_;
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
      publisher_ = null;
      context_ = context;

      facts_ = new Dictionary<string, string>();
      InitFacts();
    }
    #endregion

    public override void Start(IRubyServiceHost service_host) {
      publisher_ = GetPublisherSocket(context_, settings_.PublisherPort);
      start_stop_event_.WaitOne();
      start_stop_event_.Close();
    }

    Socket GetPublisherSocket(Context context, int port) {
      Socket socket = context.Socket(SocketType.PUB);
      socket.Bind("tcp://*:" + port);
      return socket;
    }

    public override void Stop(IRubyMessage message) {
      start_stop_event_.Set();
      publisher_.Dispose();
      context_.Dispose();
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
          .SetKey("stack-trace")
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
        publisher_.SendMore(message.Application, Encoding.UTF8);
        publisher_.Send(message.ToByteArray());
      } catch (System.Exception exception) {
        Store(GetInternalLogMessage(exception));
      }
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
      } catch(System.Exception exception) {
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
