using System;
using System.Collections.Generic;
using Nohros.Concurrent;
using Nohros.Ruby.Protocol;
using Nohros.Ruby.Protocol.Control;
using ZMQ;
using R = Nohros.Resources.StringResources;

namespace Nohros.Ruby
{
  internal class SelfHostProcess : AbstractRubyProcess
  {
    class Tracker
    {
      #region .ctor
      public Tracker() {
      }

      public Tracker(TrackerMessageChannel channel) {
        MessageChannel = channel;
      }
      #endregion

      /// <summary>
      /// Gets or sest the <see cref="TrackerMessageChannel"/> that is used
      /// to send messages to a tracker.
      /// </summary>
      public TrackerMessageChannel MessageChannel { get; set; }

      /// <summary>
      /// Gets or sets the date and time the tracker was last seen.
      /// </summary>
      /// <remarks>
      /// A tracker sends a beacon at regular intervals to mark its presence.
      /// This property register the date and time when the last beacon was
      /// received by a tracker.
      /// </remarks>
      public DateTime LastSeen { get; set; }
    }

    const string kClassName = "Nohros.Ruby.SelfHostProcess";
    readonly RubyLogger logger_;

    readonly Dictionary<int, QueryMessage> queries_;
    readonly SelfHostMessageChannel self_host_message_channel_;
    readonly IServicesRepository services_repository_;
    readonly Trackers trackers_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="SelfHostProcess"/>
    /// using the specified application settings and IPC communication channel.
    /// </summary>
    /// <param name="settings">
    /// A <see cref="IRubySettings"/> containing the user defined application
    /// settings.
    /// </param>
    /// <param name="self_host_message_channel">
    /// A <see cref="SelfHostMessageChannel"/> that provides a link between the
    /// internal and external processes.
    /// </param>
    public SelfHostProcess(IRubySettings settings,
      SelfHostMessageChannel self_host_message_channel, Trackers trackers,
      IServicesRepository services_repository)
      : base(settings, self_host_message_channel) {
      self_host_message_channel_ = self_host_message_channel;
      trackers_ = trackers;
      services_repository_ = services_repository;
      logger_ = RubyLogger.ForCurrentProcess;
    }
    #endregion

    /// <inheritdoc/>
    public override void Run(string command_line_string) {
      // Open the channels before calling the base Run method.
      self_host_message_channel_.BeaconReceived += trackers_.OnBeaconReceived;
      self_host_message_channel_.MessagePacketReceived +=
        OnMessagePacketReceived;
      self_host_message_channel_.MessagePacketSent += OnMessagePacketSent;
      self_host_message_channel_.Open();
      base.Run(command_line_string);
    }

    public void OnMessagePacketSent(RubyMessagePacket packet) {
      switch (packet.Message.Type) {
        case (int) NodeMessageType.kNodeQuery:
          QueryService(packet);
          break;
      }
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
