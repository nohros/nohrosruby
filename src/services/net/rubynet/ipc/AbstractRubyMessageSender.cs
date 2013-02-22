using System;
using System.Collections.Generic;
using Google.ProtocolBuffers;
using Nohros.Ruby.Protocol;

namespace Nohros.Ruby
{
  public abstract class AbstractRubyMessageSender : IRubyMessageSender
  {
    /// <inheritdoc/>
    public bool Send(byte[] message_id, int type, byte[] message,
      byte[] destination) {
      return Send(message_id, type, message, destination, string.Empty);
    }

    public bool Send(byte[] message_id, int type, byte[] message,
      byte[] destination, IEnumerable<KeyValuePair<string, string>> facts) {
      return Send(message_id, type, message, destination, string.Empty, facts);
    }

    /// <inheritdoc/>
    public bool Send(byte[] message_id, int type, byte[] message,
      byte[] destination, string token) {
      return Send(message_id, type, message, destination, token,
        new KeyValuePair<string, string>[0]);
    }

    public bool Send(byte[] message_id, int type, byte[] message,
      byte[] destination, string token,
      IEnumerable<KeyValuePair<string, string>> facts) {
      RubyMessage request = new RubyMessage.Builder()
        .SetId(ByteString.CopyFrom(message_id))
        .SetMessage(ByteString.CopyFrom(message))
        .SetType(type)
        .SetSender(ByteString.CopyFrom(destination))
        .Build();
      return Send(request, facts);
    }

    /// <inheritdoc/>
    public virtual bool Send(IRubyMessage message) {
      return Send(message, new KeyValuePair<string, string>[0]);
    }

    /// <inheritdoc/>
    public virtual bool Send(IRubyMessage message,
      IEnumerable<KeyValuePair<string, string>> facts) {
#if DEBUG
      if (!channel_is_opened_) {
        logger_.Warn("Send() called on a closed channel.");
        return false;
      }
#endif
      // Send the message to the service for processing.
      RubyMessage response = message as RubyMessage ??
        RubyMessage.ParseFrom(message.ToByteArray());

      // Create the reply packed using the service processing result.
      int message_size = message.ToByteArray().Length;
      RubyMessageHeader header = new RubyMessageHeader.Builder()
        .SetId(ByteString.CopyFrom(message.Id))
        .SetSize(message_size)
        .AddRangeFacts(KeyValuePairs.FromKeyValuePairs(facts))
        .Build();

      int header_size = header.SerializedSize;
      RubyMessagePacket packet = new RubyMessagePacket.Builder()
        .SetHeader(header)
        .SetHeaderSize(header.SerializedSize)
        .SetMessage(response)
        .SetSize(header_size + 2 + message_size)
        .Build();
      return Send(packet);
    }

    public abstract bool Send(RubyMessagePacket packet);
  }
}
