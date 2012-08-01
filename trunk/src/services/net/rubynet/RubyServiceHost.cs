using System;
using System.Collections.Generic;
using Nohros.Concurrent;
using Nohros.Configuration;
using Nohros.Data.Json;
using Nohros.Logging;
using Nohros.Ruby.Protocol;
using Nohros.Ruby.Protocol.Control;

namespace Nohros.Ruby
{
  /// <summary>
  /// .NET implementation of the <see cref="IRubyServiceHost"/> interface. This
  /// class is used to host a .NET based ruby services.
  /// </summary>
  internal class RubyServiceHost : IRubyServiceHost, IRubyMessageListener
  {
    const string kClassName = "Nohros.Ruby.RubyServiceHost";
    readonly RubyLogger logger_ = RubyLogger.ForCurrentProcess;

    readonly IRubyMessageChannel ruby_message_channel_;
    readonly IRubyService service_;
    readonly IRubySettings settings_;
    readonly IRubyLogger service_logger_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="RubyServiceHost"/> class
    /// by using the specified service to hostm message sender and listener.
    /// </summary>
    /// <param name="service">
    /// The service to host.
    /// </param>
    /// <param name="channel">
    /// A <see cref="IRubyMessageSender"/> that can be used to send messages to
    /// the ruby service node.
    /// </param>
    public RubyServiceHost(IRubyService service, IRubyMessageChannel channel,
      IRubySettings settings) {
#if DEBUG
      if (service == null || channel == null) {
        throw new ArgumentNullException(service == null ? "service" : "sender");
      }
#endif
      service_ = service;
      ruby_message_channel_ = channel;
      settings_ = settings;
      service_logger_ = new RubyLogger(
        new AggregatorLogger(
          ProviderOptions.GetIfExists(service.Facts,
            StringResources.kServiceNameFact,
            Strings.kNodeServiceName), settings));
    }
    #endregion

    public void OnMessagePacketReceived(RubyMessagePacket packet) {
      if (logger_.IsDebugEnabled) {
        logger_.Debug("Received a message with token: "
          + packet.Message.HasToken);
      }
      // send the message to the service for processing.
      service_.OnMessage(packet.Message);
    }

    /// <inheritdoc/>
    public bool Send(IRubyMessage message) {
      if (logger_.IsDebugEnabled) {
        logger_.Debug("Sending a message with token " + message.Token);
      }
      return ruby_message_channel_.Send(message);
    }

    /// <inherithdoc/>
    public IRubyLogger Logger {
      get { return service_logger_; }
    }

    /// <summary>
    /// Starts the hosted service.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The hosted service runs into a dedicated thread. The thread where
    /// this code is running is used to send/receive messages to/from the
    /// service.
    /// </para>
    /// <para>
    /// This method does not return until the running hosted service have
    /// finished your execution.
    /// </para>
    /// <para>
    /// If the service throws any exception this is propaggated to the
    /// caller and the service is forced to stop.
    /// </para>
    /// </remarks>
    public void Start() {
      ruby_message_channel_.AddListener(this, Executors.ThreadPoolExecutor());
      Announce();
      service_.Start(this);
    }

    void Announce() {
      // Tell the service node that we are hosting a new service.
      AnnounceMessage.Builder builder = new AnnounceMessage.Builder();
      foreach (KeyValuePair<string, string> fact in service_.Facts) {
        builder.AddFacts(
          new KeyValuePair.Builder()
            .SetKey(fact.Key)
            .SetValue(fact.Value));
      }

      RubyMessage message = new RubyMessage.Builder()
        .SetId(0)
        .SetToken(StringResources.kAnnounceMessageToken)
        .SetType((int) NodeMessageType.kNodeAnnounce)
        .SetMessage(builder.Build().ToByteString())
        .Build();
      Send(message);

      if (logger_.IsDebugEnabled) {
        JsonStringBuilder json = new JsonStringBuilder().WriteBeginObject();
        foreach (KeyValuePair<string, string> fact in service_.Facts) {
          json.WriteMember(fact.Key, fact.Value);
        }
        json.WriteEndObject();
        logger_.Debug("Announcing service: " + json);
      }
    }

    /// <inherithdoc/>
    public IRubyService Service {
      get { return service_; }
    }
  }
}
