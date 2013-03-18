using System;
using System.Collections.Generic;
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
    public static RubyMessagePacket CreateMessagePacket(byte[] message_id,
      int type, byte[] message, IEnumerable<string> extra_info) {
      return CreateMessagePacket(message_id, type, message, extra_info,
        new KeyValuePair<string, string>[0]);
    }

    public static RubyMessagePacket CreateMessagePacket(byte[] message_id,
      int type, byte[] message, IEnumerable<string> extra_info, string token) {
      return CreateMessagePacket(message_id, type, message, extra_info, token,
        new KeyValuePair<string, string>[0]);
    }

    public static RubyMessagePacket CreateMessagePacket(byte[] message_id,
      int type, byte[] message, IEnumerable<string> extra_info,
      byte[] destination) {
      return CreateMessagePacket(message_id, type, message, extra_info,
        destination, new KeyValuePair<string, string>[0]);
    }

    public static RubyMessagePacket CreateMessagePacket(byte[] message_id,
      int type, byte[] message, IEnumerable<string> extra_info, string token,
      byte[] destination) {
      return CreateMessagePacket(message_id, type, message, extra_info, token,
        destination, new KeyValuePair<string, string>[0]);
    }

    public static RubyMessagePacket CreateMessagePacket(byte[] message_id,
      int type, byte[] message, IEnumerable<string> extra_info,
      IEnumerable<KeyValuePair<string, string>> facts) {
      return CreateMessagePacket(ByteString.CopyFrom(message_id), type,
        ByteString.CopyFrom(message), extra_info, facts);
    }

    public static RubyMessagePacket CreateMessagePacket(byte[] message_id,
      int type, byte[] message, IEnumerable<string> extra_info, string token,
      IEnumerable<KeyValuePair<string, string>> facts) {
      return CreateMessagePacket(ByteString.CopyFrom(message_id), type,
        ByteString.CopyFrom(message), extra_info, token, facts);
    }

    public static RubyMessagePacket CreateMessagePacket(byte[] message_id,
      int type, byte[] message, IEnumerable<string> extra_info,
      byte[] destination,
      IEnumerable<KeyValuePair<string, string>> facts) {
      return CreateMessagePacket(ByteString.CopyFrom(message_id), type,
        ByteString.CopyFrom(message), extra_info,
        ByteString.CopyFrom(destination), facts);
    }

    public static RubyMessagePacket CreateMessagePacket(byte[] message_id,
      int type, byte[] message, IEnumerable<string> extra_info, string token,
      byte[] destination,
      IEnumerable<KeyValuePair<string, string>> facts) {
      return CreateMessagePacket(ByteString.CopyFrom(message_id), type,
        ByteString.CopyFrom(message), extra_info, token,
        ByteString.CopyFrom(destination), facts);
    }

    public static RubyMessagePacket CreateMessagePacket(ByteString message_id,
      int type, ByteString message, IEnumerable<string> extra_info) {
      return CreateMessagePacket(message_id, type, message, extra_info,
        new KeyValuePair<string, string>[0]);
    }

    public static RubyMessagePacket CreateMessagePacket(ByteString message_id,
      int type, ByteString message, IEnumerable<string> extra_info, string token) {
      return CreateMessagePacket(message_id, type, message, extra_info, token,
        new KeyValuePair<string, string>[0]);
    }

    public static RubyMessagePacket CreateMessagePacket(ByteString message_id,
      int type, ByteString message, IEnumerable<string> extra_info,
      ByteString destination) {
      return CreateMessagePacket(message_id, type, message, extra_info,
        destination, new KeyValuePair<string, string>[0]);
    }

    public static RubyMessagePacket CreateMessagePacket(ByteString message_id,
      int type, ByteString message, IEnumerable<string> extra_info, string token,
      ByteString destination) {
      return CreateMessagePacket(message_id, type, message, extra_info, token,
        destination, new KeyValuePair<string, string>[0]);
    }

    public static RubyMessagePacket CreateMessagePacket(ByteString message_id,
      int type, ByteString message, IEnumerable<string> extra_info,
      IEnumerable<KeyValuePair<string, string>> facts) {
      RubyMessage msg = CreateMessage(message_id, type, message, extra_info);
      return CreateMessagePacket(msg, facts);
    }

    public static RubyMessagePacket CreateMessagePacket(ByteString message_id,
      int type, ByteString message, IEnumerable<string> extra_info, string token,
      IEnumerable<KeyValuePair<string, string>> facts) {
      RubyMessage msg = CreateMessage(message_id, type, message, extra_info,
        token);
      return CreateMessagePacket(msg, facts);
    }

    public static RubyMessagePacket CreateMessagePacket(ByteString message_id,
      int type, ByteString message, IEnumerable<string> extra_info,
      ByteString destination,
      IEnumerable<KeyValuePair<string, string>> facts) {
      RubyMessage msg = CreateMessage(message_id, type, message, extra_info,
        destination);
      return CreateMessagePacket(msg, facts);
    }

    public static RubyMessagePacket CreateMessagePacket(ByteString message_id,
      int type, ByteString message, IEnumerable<string> extra_info, string token,
      ByteString destination, IEnumerable<KeyValuePair<string, string>> facts) {
      RubyMessage msg = CreateMessage(message_id, type, message, extra_info,
        token, destination);
      return CreateMessagePacket(msg, facts);
    }


    public static RubyMessage CreateMessage(byte[] message_id,
      int type, byte[] message, IEnumerable<string> extra_info) {
      return CreateMessage(ByteString.CopyFrom(message_id), type,
        ByteString.CopyFrom(message), extra_info);
    }

    public static RubyMessage CreateMessage(byte[] message_id,
      int type, byte[] message, IEnumerable<string> extra_info, string token) {
      return CreateMessage(ByteString.CopyFrom(message_id), type,
        ByteString.CopyFrom(message), extra_info, token);
    }

    public static RubyMessage CreateMessage(byte[] message_id,
      int type, byte[] message, IEnumerable<string> extra_info,
      byte[] destination) {
      return CreateMessage(ByteString.CopyFrom(message_id), type,
        ByteString.CopyFrom(message), extra_info,
        ByteString.CopyFrom(destination));
    }

    public static RubyMessage CreateMessage(byte[] message_id,
      int type, byte[] message, IEnumerable<string> extra_info, string token,
      byte[] destination) {
      return CreateMessage(ByteString.CopyFrom(message_id), type,
        ByteString.CopyFrom(message), extra_info,
        ByteString.CopyFrom(destination));
    }

    public static RubyMessage CreateMessage(ByteString message_id,
      int type, ByteString message, IEnumerable<string> extra_info) {
      return CreateMessageBuilder(message_id, type, message, extra_info)
        .Build();
    }

    public static RubyMessage CreateMessage(ByteString message_id,
      int type, ByteString message, IEnumerable<string> extra_info, string token) {
      return CreateMessageBuilder(message_id, type, message, extra_info)
        .SetToken(token)
        .Build();
    }

    public static RubyMessage CreateMessage(ByteString message_id,
      int type, ByteString message, IEnumerable<string> extra_info,
      ByteString destination) {
      return CreateMessageBuilder(message_id, type, message, extra_info)
        .SetSender(destination)
        .Build();
    }

    public static RubyMessage CreateMessage(ByteString message_id,
      int type, ByteString message, IEnumerable<string> extra_info, string token,
      ByteString destination) {
      return CreateMessageBuilder(message_id, type, message, extra_info)
        .SetToken(token)
        .SetSender(destination)
        .Build();
    }

    static RubyMessage.Builder CreateMessageBuilder(ByteString message_id,
      int type, ByteString message, IEnumerable<string> extra_info) {
      return CreateMessageBuilder(message_id, type, message)
        .AddRangeExtraInfo(extra_info);
    }


    public static RubyMessagePacket CreateMessagePacket(byte[] message_id,
      int type, byte[] message) {
      return CreateMessagePacket(message_id, type, message,
        new KeyValuePair<string, string>[0]);
    }

    public static RubyMessagePacket CreateMessagePacket(byte[] message_id,
      int type, byte[] message, string token) {
      return CreateMessagePacket(message_id, type, message, token,
        new KeyValuePair<string, string>[0]);
    }

    public static RubyMessagePacket CreateMessagePacket(byte[] message_id,
      int type, byte[] message, byte[] destination) {
      return CreateMessagePacket(message_id, type, message, destination,
        new KeyValuePair<string, string>[0]);
    }

    public static RubyMessagePacket CreateMessagePacket(byte[] message_id,
      int type, byte[] message, string token, byte[] destination) {
      return CreateMessagePacket(message_id, type, message, token,
        destination, new KeyValuePair<string, string>[0]);
    }

    public static RubyMessagePacket CreateMessagePacket(byte[] message_id,
      int type, byte[] message, IEnumerable<KeyValuePair<string, string>> facts) {
      return CreateMessagePacket(ByteString.CopyFrom(message_id), type,
        ByteString.CopyFrom(message), facts);
    }

    public static RubyMessagePacket CreateMessagePacket(byte[] message_id,
      int type, byte[] message, string token,
      IEnumerable<KeyValuePair<string, string>> facts) {
      return CreateMessagePacket(ByteString.CopyFrom(message_id), type,
        ByteString.CopyFrom(message), token, facts);
    }

    public static RubyMessagePacket CreateMessagePacket(byte[] message_id,
      int type, byte[] message, byte[] destination,
      IEnumerable<KeyValuePair<string, string>> facts) {
      return CreateMessagePacket(ByteString.CopyFrom(message_id), type,
        ByteString.CopyFrom(message), ByteString.CopyFrom(destination), facts);
    }

    public static RubyMessagePacket CreateMessagePacket(byte[] message_id,
      int type, byte[] message, string token, byte[] destination,
      IEnumerable<KeyValuePair<string, string>> facts) {
      return CreateMessagePacket(ByteString.CopyFrom(message_id), type,
        ByteString.CopyFrom(message), token, ByteString.CopyFrom(destination),
        facts);
    }

    public static RubyMessagePacket CreateMessagePacket(ByteString message_id,
      int type, ByteString message) {
      return CreateMessagePacket(message_id, type, message,
        new KeyValuePair<string, string>[0]);
    }

    public static RubyMessagePacket CreateMessagePacket(ByteString message_id,
      int type, ByteString message, string token) {
      return CreateMessagePacket(message_id, type, message, token,
        new KeyValuePair<string, string>[0]);
    }

    public static RubyMessagePacket CreateMessagePacket(ByteString message_id,
      int type, ByteString message, ByteString destination) {
      return CreateMessagePacket(message_id, type, message, destination,
        new KeyValuePair<string, string>[0]);
    }

    public static RubyMessagePacket CreateMessagePacket(ByteString message_id,
      int type, ByteString message, string token, ByteString destination) {
      return CreateMessagePacket(message_id, type, message, token,
        destination, new KeyValuePair<string, string>[0]);
    }

    public static RubyMessagePacket CreateMessagePacket(ByteString message_id,
      int type, ByteString message,
      IEnumerable<KeyValuePair<string, string>> facts) {
      RubyMessage msg = CreateMessage(message_id, type, message);
      return CreateMessagePacket(msg, facts);
    }

    public static RubyMessagePacket CreateMessagePacket(ByteString message_id,
      int type, ByteString message, string token,
      IEnumerable<KeyValuePair<string, string>> facts) {
      RubyMessage msg = CreateMessage(message_id, type, message, token);
      return CreateMessagePacket(msg, facts);
    }

    public static RubyMessagePacket CreateMessagePacket(ByteString message_id,
      int type, ByteString message, ByteString destination,
      IEnumerable<KeyValuePair<string, string>> facts) {
      RubyMessage msg = CreateMessage(message_id, type, message, destination);
      return CreateMessagePacket(msg, facts);
    }

    public static RubyMessagePacket CreateMessagePacket(ByteString message_id,
      int type, ByteString message, string token, ByteString destination,
      IEnumerable<KeyValuePair<string, string>> facts) {
      RubyMessage msg = CreateMessage(message_id, type, message, token,
        destination);
      return CreateMessagePacket(msg, facts);
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

    public static RubyMessage CreateMessage(byte[] message_id,
      int type, byte[] message) {
      return CreateMessage(ByteString.CopyFrom(message_id), type,
        ByteString.CopyFrom(message));
    }

    public static RubyMessage CreateMessage(byte[] message_id,
      int type, byte[] message, string token) {
      return CreateMessage(ByteString.CopyFrom(message_id), type,
        ByteString.CopyFrom(message), token);
    }

    public static RubyMessage CreateMessage(byte[] message_id,
      int type, byte[] message, byte[] destination) {
      return CreateMessage(ByteString.CopyFrom(message_id), type,
        ByteString.CopyFrom(message), ByteString.CopyFrom(destination));
    }

    public static RubyMessage CreateMessage(byte[] message_id,
      int type, byte[] message, string token, byte[] destination) {
      return CreateMessage(ByteString.CopyFrom(message_id), type,
        ByteString.CopyFrom(message), ByteString.CopyFrom(destination));
    }

    public static RubyMessage CreateMessage(ByteString message_id,
      int type, ByteString message) {
      return CreateMessageBuilder(message_id, type, message).Build();
    }

    public static RubyMessage CreateMessage(ByteString message_id,
      int type, ByteString message, string token) {
      return CreateMessageBuilder(message_id, type, message)
        .SetToken(token)
        .Build();
    }

    public static RubyMessage CreateMessage(ByteString message_id,
      int type, ByteString message, ByteString destination) {
      return CreateMessageBuilder(message_id, type, message)
        .SetSender(destination)
        .Build();
    }

    public static RubyMessage CreateMessage(ByteString message_id,
      int type, ByteString message, string token, ByteString destination) {
      return CreateMessageBuilder(message_id, type, message)
        .SetToken(token)
        .SetSender(destination)
        .Build();
    }

    static RubyMessage.Builder CreateMessageBuilder(ByteString message_id,
      int type,
      ByteString message) {
      return new RubyMessage.Builder()
        .SetId(message_id)
        .SetType(type)
        .SetMessage(message);
    }
  }
}
