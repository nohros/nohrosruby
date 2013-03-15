using System.Collections.Generic;
using Google.ProtocolBuffers;
using Nohros.Ruby.Protocol;
using ZMQ;
using ZmqSocket = ZMQ.Socket;
using Exception = System.Exception;
using R = Nohros.Resources.StringResources;

namespace Nohros.Ruby
{
  public abstract class AbstractRubyMessageSender : IRubyMessageSender
  {
    /// <inheritdoc/>
    public virtual bool Send(IRubyMessage message) {
      return Send(message, new KeyValuePair<string, string>[0]);
    }

    /// <inheritdoc/>
    public virtual bool Send(IRubyMessage message,
      IEnumerable<KeyValuePair<string, string>> facts) {
      // Send the message to the service for processing.
      RubyMessage response = message as RubyMessage ??
        RubyMessage.ParseFrom(message.ToByteArray());
      return Send(RubyMessages.CreateMessagePacket(response));
    }

    /// <inheritdoc/>
    public bool Send(byte[] message_id, int type, byte[] message) {
      return Send(message_id, type, message, new KeyValuePair<string, string>[0]);
    }

    /// <inheritdoc/>
    public bool Send(byte[] message_id, int type, byte[] message, string token) {
      return Send(message_id, type, message, token,
        new KeyValuePair<string, string>[0]);
    }

    /// <inheritdoc/>
    public bool Send(byte[] message_id, int type, byte[] message,
      byte[] destination) {
      return Send(message_id, type, message, destination,
        new KeyValuePair<string, string>[0]);
    }

    /// <inheritdoc/>
    public bool Send(byte[] message_id, int type, byte[] message,
      string token, byte[] destination) {
      return Send(message_id, type, message, token, destination,
        new KeyValuePair<string, string>[0]);
    }

    public bool Send(byte[] message_id, int type, byte[] message,
      IEnumerable<KeyValuePair<string, string>> facts) {
      RubyMessage request = RubyMessages.CreateMessage(message_id, type,
        message);
      return Send(request, facts);
    }

    public bool Send(byte[] message_id, int type, byte[] message, string token,
      IEnumerable<KeyValuePair<string, string>> facts) {
      RubyMessage request = RubyMessages.CreateMessage(message_id, type,
        message, token);
      return Send(request, facts);
    }

    public bool Send(byte[] message_id, int type, byte[] message,
      byte[] destination, IEnumerable<KeyValuePair<string, string>> facts) {
      RubyMessage request = RubyMessages.CreateMessage(message_id, type,
        message, destination);
      return Send(request, facts);
    }

    public bool Send(byte[] message_id, int type, byte[] message, string token,
      byte[] destination, IEnumerable<KeyValuePair<string, string>> facts) {
      RubyMessage request = RubyMessages.CreateMessage(message_id, type,
        message, token, destination);
      return Send(request, facts);
    }

    public abstract bool Send(RubyMessagePacket packet);
  }
}
