using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Google.ProtocolBuffers;
using Nohros.Ruby.Protocol;

namespace Nohros.Ruby
{
  /// <summary>
  /// A collection of utility methods for classes related with the
  /// <see cref="RubyMessage"/> class.
  /// </summary>
  public sealed class RubyMessages
  {
    public static RubyMessagePacket CreateMessagePacket(byte[] message_id, int type,
      byte[] message) {
      return CreateMessagePacket(message_id, type, message, string.Empty);
    }

    public static RubyMessagePacket CreateMessagePacket(byte[] message_id, int type,
      byte[] message, byte[] destination) {
      return CreateMessagePacket(message_id, type, message, destination,
        string.Empty);
    }

    public static RubyMessagePacket CreateMessagePacket(byte[] message_id, int type,
      byte[] message, IEnumerable<KeyValuePair<string, string>> facts) {
      return CreateMessagePacket(message_id, type, message, string.Empty, facts);
    }

    public static RubyMessagePacket CreateMessagePacket(byte[] message_id, int type,
      byte[] message, byte[] destination,
      IEnumerable<KeyValuePair<string, string>> facts) {
      return CreateMessagePacket(message_id, type, message, destination,
        string.Empty, facts);
    }


    public static RubyMessagePacket CreateMessagePacket(byte[] message_id, int type,
      byte[] message, string token) {
      return CreateMessagePacket(message_id, type, message, token,
        new KeyValuePair<string, string>[0]);
    }

    public static RubyMessagePacket CreateMessagePacket(byte[] message_id, int type,
      byte[] message, byte[] destination, string token) {
      return CreateMessagePacket(message_id, type, message, destination, token,
        new KeyValuePair<string, string>[0]);
    }

    public static RubyMessagePacket CreateMessagePacket(byte[] message_id, int type,
      byte[] message, string token,
      IEnumerable<KeyValuePair<string, string>> facts) {
      RubyMessage request = new RubyMessage.Builder()
        .SetId(ByteString.CopyFrom(message_id))
        .SetMessage(ByteString.CopyFrom(message))
        .SetType(type)
        .Build();
      return CreateMessagePacket(request, facts);
    }

    public static RubyMessagePacket CreateMessagePacket(byte[] message_id, int type,
      byte[] message, byte[] destination, string token,
      IEnumerable<KeyValuePair<string, string>> facts) {
      RubyMessage request = new RubyMessage.Builder()
        .SetId(ByteString.CopyFrom(message_id))
        .SetMessage(ByteString.CopyFrom(message))
        .SetType(type)
        .SetSender(ByteString.CopyFrom(destination))
        .Build();
      return CreateMessagePacket(request, facts);
    }

    public static RubyMessagePacket CreateMessagePacket(RubyMessage message) {
      return CreateMessagePacket(message, new KeyValuePair<string, string>[0]);
    }

    public static RubyMessagePacket CreateMessagePacket(RubyMessage message,
      IEnumerable<KeyValuePair<string, string>> facts) {
      int message_size = message.ToByteArray().Length;
      RubyMessageHeader header = new RubyMessageHeader.Builder()
        .SetId(message.Id)
        .SetSize(message_size)
        .AddRangeFacts(KeyValuePairs.FromKeyValuePairs(facts))
        .Build();

      int header_size = header.SerializedSize;
      RubyMessagePacket packet = new RubyMessagePacket.Builder()
        .SetHeader(header)
        .SetHeaderSize(header.SerializedSize)
        .SetMessage(message)
        .SetSize(header_size + 2 + message_size)
        .Build();
      return packet;
    }
  }
}
