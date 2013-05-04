using System;
using System.Collections.Generic;
using Nohros.Ruby.Extensions;
using Nohros.Ruby.Protocol;
using Nohros.Ruby.Protocol.Control;
using R = Nohros.Resources.StringResources;

namespace Nohros.Ruby
{
  internal class SelfHostProcess : AbstractRubyProcess
  {
    const string kClassName = "Nohros.Ruby.SelfHostProcess";
    readonly HostMessageChannel host_message_channel_;
    readonly RubyLogger logger_;

    readonly Dictionary<int, QueryMessage> queries_;
    readonly RubySettings settings_;
    readonly TrackerEngine trackers_;
    readonly List<IRubyServiceHost> hosts_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="SelfHostProcess"/>
    /// using the specified application settings and IPC communication channel.
    /// </summary>
    /// <param name="settings">
    /// A <see cref="IRubySettings"/> containing the user defined application
    /// settings.
    /// </param>
    /// <param name="host_message_channel">
    /// A <see cref="HostMessageChannel"/> that provides a link between the
    /// internal and external processes.
    /// </param>
    public SelfHostProcess(RubySettings settings,
      HostMessageChannel host_message_channel, TrackerEngine trackers)
      : base(settings, host_message_channel) {
      host_message_channel_ = host_message_channel;
      trackers_ = trackers;
      logger_ = RubyLogger.ForCurrentProcess;
      settings_ = settings;
      hosts_ = new List<IRubyServiceHost>();
    }
    #endregion

    /// <inheritdoc/>
    public override void Run(string command_line_string) {
      logger_.Info("self host process has been started");

      // Open the channels before calling the base Run method.
      host_message_channel_.MailboxMessagePacketReceived +=
        OnMessagePacketReceived;
      host_message_channel_.MailboxMessagePacketReceived +=
        trackers_.OnMessagePacketReceived;
      host_message_channel_.MessagePacketSent += OnMessagePacketSent;
      host_message_channel_.MailboxBind += OnMailboxBind;
      host_message_channel_.Open();

      trackers_.TrackerDiscovered += OnTrackerDiscovered;
    }

    void OnMailboxBind(ZMQEndPoint endpoint) {
      trackers_.Start(host_message_channel_.Endpoint);
    }

    /// <inheritdoc/>
    public override void OnMessagePacketReceived(RubyMessagePacket packet) {
      // TODO: Filter the message based on the facts.
      base.OnMessagePacketReceived(packet);
      host_message_channel_.OnMessagePacketReceived(packet);
    }

    public void OnMessagePacketSent(RubyMessagePacket packet) {
      switch (packet.Message.Type) {
        case (int) NodeMessageType.kNodeQuery:
          QueryService(packet);
          break;

        case (int) NodeMessageType.kNodeAnnounce:
          Announce(packet);
          break;
      }
    }

    public override void Exit() {
      trackers_.Shutdown();
    }

    void QueryService(RubyMessagePacket packet) {
      QueryMessage message = QueryMessage.ParseFrom(packet.Message.Message);
      trackers_.FindServices(KeyValuePairs.ToKeyValuePairs(message.FactsList),
        endpoint => {
          var response = CreateMessagePacket(packet.Message.Id.ToByteArray(),
            endpoint);
          OnMessagePacketReceived(response);
        });
    }

    void Announce(RubyMessagePacket packet) {
      // send the announcement to the trackers.
      AnnounceMessage message = AnnounceMessage.ParseFrom(packet.Message.Message);
      trackers_.Announce(message.FactsList);
    }

    void OnTrackerDiscovered(Tracker tracker) {
      // sync our services database with the tracker.
      foreach (var host in hosts_) {
        trackers_.Announce(host.Service.Facts.FromKeyValuePairs(), tracker);
      }
    }

    /// <summary>
    /// Method taht should be called when a <see cref="IRubyServiceHost"/> is
    /// started.
    /// </summary>
    /// <param name="host">
    /// The <see cref="IRubyServiceHost"/> that has been started.
    /// </param>
    /// <remarks>
    /// <see cref="SelfHostProcess"/> does not start services for itself, this
    /// operation should be performed by the class that is using the
    /// <see cref="SelfHostProcess"/> object. The
    /// <see cref="OnServiceHostStart"/> method allows the service that is
    /// using the <see cref="SelfHostProcess"/> object to signal about the
    /// host tha has been started. This allows the self host class to perform
    /// its operation over the hosted service(i.e announce, find service)
    /// </remarks>
    internal void OnServiceHostStart(IRubyServiceHost host) {
      lock(hosts_) {
        hosts_.Add(host);
      }
    }

    RubyMessagePacket CreateMessagePacket(byte[] request_id,
      ZMQEndPoint endpoint) {
      ResponseMessage response = new ResponseMessage.Builder()
        .AddReponses(KeyValuePairs.FromKeyValuePair(
          Strings.kServiceEndpointFact, endpoint.Endpoint))
        .Build();
      return RubyMessages.CreateMessagePacket(request_id,
        (int) NodeMessageType.kNodeResponse, response.ToByteArray());
    }
  }
}
