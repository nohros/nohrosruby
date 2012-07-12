using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ZMQ;

using Nohros.Ruby;
using Nohros.Ruby.Protocol;
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

    readonly IDictionary<string, string> facts_;

    readonly IRubyLogger logger_ = RubyLogger.ForCurrentProcess;
    readonly Mailbox<LogMessage> mailbox_;
    readonly Context context_;
    Socket dealer_, publisher_;
    readonly IAggregatorSettings settings_;
    bool is_running_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="Aggregator"/> class
    /// by using the specified ZMQ sockets.
    /// </summary>
    public Aggregator(Context context, IAggregatorSettings settings) {
      settings_ = settings;
      mailbox_ = new Mailbox<LogMessage>(OnLogMessage);
      context_ = context;
      is_running_ = false;
      dealer_ = null;
      publisher_ = null;
      context_ = context;

      facts_ = new Dictionary<string, string>();
      InitFacts();
    }
    #endregion

    public override void Start(IRubyServiceHost service_host) {
      publisher_ = GetPublisherSocket(context_, settings_.PublisherPort);
      dealer_ = GetDealerSocket(context_, settings_.ListenerPort);
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

    Socket GetDealerSocket(Context context, int port) {
      Socket socket = context.Socket(SocketType.DEALER);
      socket.Bind("tcp://*:" + port);
      return socket;
    }

    Socket GetPublisherSocket(Context context, int port) {
      Socket socket = context.Socket(SocketType.PUB);
      socket.Bind("tcp://*:" + port);
      return socket;
    }

    public override void Stop(IRubyMessage message) {
      is_running_ = false;
      dealer_.Dispose();
      publisher_.Dispose();
      context_.Dispose();
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
      publisher_.SendMore(message.Application, Encoding.UTF8);
      publisher_.Send(message.ToByteArray());
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
