using System;
using System.Collections.Generic;
using System.Text;
using Nohros.Ruby.Protocol;
using ZMQ;
using Nohros.Concurrent;
using Nohros.Resources;
using Nohros.Data.Json;

namespace Nohros.Ruby.Logging
{
  /// <summary>
  /// A class that aggregates log messages send from anywhere (possible from a
  /// service) publish and persist them to a database.
  /// </summary>
  public class Aggregator : AbstractRubyService
  {
    const string kClassName = "Nohros.Ruby.Logging.Aggregator";

    readonly Socket dealer_;
    readonly IDictionary<string, string> facts_;

    readonly IAggregatorLogger logger_ = AggregatorLogger.ForCurrentProcess;
    readonly Mailbox<LogMessage> mailbox_;
    readonly Socket publisher_;
    readonly IAggregatorSettings settings_;
    bool is_running_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="Aggregator"/> class
    /// by using the specified ZMQ sockets.
    /// </summary>
    /// <param name="dealer">
    /// A <see cref="Socket"/> of type <see cref="SocketType.DEALER"/> that is
    /// used to receive messages from somewhere.
    /// </param>
    /// <param name="publisher">
    /// A <see cref="Socket"/> of type <see cref="SocketType.PUB"/> that is
    /// used to publish the message.
    /// </param>
    public Aggregator(Socket dealer, Socket publisher,
      IAggregatorSettings settings) {
      publisher_ = publisher;
      dealer_ = dealer;
      settings_ = settings;
      mailbox_ = new Mailbox<LogMessage>(OnLogMessage);
      is_running_ = false;

      facts_ = new Dictionary<string, string>();
      InitFacts();
    }
    #endregion

    public override void Start(IRubyServiceHost service_host) {
      is_running_ = true;
      while (is_running_) {
        try {
          byte[] data = dealer_.Recv();

          LogMessage message = LogMessage.ParseFrom(data);
          logger_.Debug("Received a message from:" + message.Application);
          mailbox_.Send(message);
        } catch (ZMQ.Exception zmqe) {
          is_running_ = false;
          if (zmqe.Errno == (int)ERRNOS.ETERM) {
            logger_.Debug("zmq::Context is terminating.");
          } else {
            logger_.Error(
              string.Format(StringResources.Log_MethodThrowsException,
                kClassName,
                "Start"), zmqe);
          }
        } catch (System.Exception exception) {
          logger_.Error(
            string.Format(StringResources.Log_MethodThrowsException, kClassName,
              "Start"), exception);
        }
      }
    }

    public override void Stop(IRubyMessage message) {
      is_running_ = false;
      dealer_.Dispose();
    }

    /// <inheritdoc/>
    public override void OnMessage(IRubyMessage message) {
    }

    void OnLogMessage(LogMessage message) {
      Publish(message);
      Store(message);
    }

    /// <summary>
    /// Publish the message to any conneted subscriber.
    /// </summary>
    /// <param name="message">
    /// The message to publish.
    /// </param>
    void Publish(LogMessage message) {
      string serialized_message = new JsonStringBuilder()
        .WriteBeginObject()
        .WriteMember("level", message.Level)
        .WriteMember("message", message.Message)
        .WriteMember("timestamp", message.TimeStamp)
        .WriteMember("exception", message.Exception)
        .WriteMember("application", message.Application)
        .ToString();
      publisher_.SendMore(message.Application, Encoding.UTF8);
      publisher_.Send(serialized_message, Encoding.UTF8);
    }

    /// <summary>
    /// Persist the messageto a database.
    /// </summary>
    /// <param name="message">
    /// The message to be persisted.
    /// </param>
    void Store(LogMessage message) {
      settings_.AggregatorDataProvider.Store(message);
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
