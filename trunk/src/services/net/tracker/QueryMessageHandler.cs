using System;
using Nohros.Ruby.Protocol;
using Nohros.Ruby.Protocol.Control;

namespace Nohros.Ruby
{
  internal class QueryMessageHandler : IMessageHandler
  {
    readonly IRubyMessageChannel ruby_message_channel_;
    readonly TrackerEngine trackers_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="QueryMessageHandler"/>
    /// class by using the specified tracker engine and message sender.
    /// </summary>
    /// <param name="trackers">
    /// A <see cref="TrackerEngine"/> that can be used to process the query.
    /// </param>
    /// <param name="ruby_message_channel">
    /// A <see cref="IRubyMessageChannel"/> that can be used to send the result
    /// of the query message processing.
    /// </param>
    public QueryMessageHandler(TrackerEngine trackers,
      IRubyMessageChannel ruby_message_channel) {
      trackers_ = trackers;
      ruby_message_channel_ = ruby_message_channel;
    }
    #endregion

    /// <inheritdoc/>
    public void Handle(IRubyMessage message) {
      QueryMessage query = QueryMessage.ParseFrom(message.Message);
      trackers_.FindServices(KeyValuePairs.ToKeyValuePairs(query.FactsList),
        endpoint => {
          var response = CreateMessagePacket(message.Id, endpoint,
            message.Sender);
          ruby_message_channel_.Send(response);
        });
    }

    RubyMessagePacket CreateMessagePacket(byte[] request_id,
      ZMQEndPoint endpoint, byte[] destination) {
      ResponseMessage response = new ResponseMessage.Builder()
        .AddAddresses(endpoint.Endpoint)
        .Build();
      return RubyMessages.CreateMessagePacket(request_id,
        (int) NodeMessageType.kNodeResponse, response.ToByteArray(), destination);
    }
  }
}
