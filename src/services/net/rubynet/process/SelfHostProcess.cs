using System;
using System.Collections.Generic;
using Nohros.Concurrent;
using Nohros.Ruby.Data;
using Nohros.Ruby.Protocol;
using Nohros.Ruby.Protocol.Control;
using ZMQ;
using R = Nohros.Resources.StringResources;

namespace Nohros.Ruby
{
  internal class SelfHostProcess : AbstractRubyProcess
  {
    const string kClassName = "Nohros.Ruby.SelfHostProcess";
    readonly RubyLogger logger_;

    readonly Dictionary<int, QueryMessage> queries_;
    readonly HostMessageChannel host_message_channel_;
    readonly TrackerEngine trackers_;

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
    public SelfHostProcess(IRubySettings settings,
      HostMessageChannel host_message_channel, TrackerEngine trackers)
      : base(settings, host_message_channel) {
      host_message_channel_ = host_message_channel;
      trackers_ = trackers;
      logger_ = RubyLogger.ForCurrentProcess;
    }
    #endregion

    /// <inheritdoc/>
    public override void Run(string command_line_string) {
      logger_.Info("self host process has been started");

      // Open the channels before calling the base Run method.
      host_message_channel_.MessagePacketReceived +=
        OnMessagePacketReceived;
      host_message_channel_.MessagePacketReceived +=
        trackers_.OnMessagePacketReceived;
      host_message_channel_.MessagePacketReceived +=
        trackers_.OnMessagePacketReceived;
      host_message_channel_.MessagePacketSent += OnMessagePacketSent;
      host_message_channel_.Open();
      trackers_.Start();
      base.Run(command_line_string);
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
      AnnounceMessage message = AnnounceMessage.ParseFrom(packet.Message.Message);
      trackers_.Announce(message.FactsList, host_message_channel_.Endpoint);
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
