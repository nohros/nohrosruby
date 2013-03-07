using System;
using System.Collections.Generic;
using Nohros.Ruby.Protocol;

namespace Nohros.Ruby
{
  /// <summary>
  /// A channel implementation that essentially behaves like "/dev/null". All
  /// calls to Send() will return <c>true</c> although no action is performed.
  /// Added listeners are ignored.
  /// </summary>
  public class NullMessageSender : IRubyMessageSender, IRubyMessageSenderFactory
  {
    /// <inheritdoc/>
    public bool Send(IRubyMessage message) {
      return true;
    }

    /// <inheritdoc/>
    public bool Send(IRubyMessage message,
      IEnumerable<KeyValuePair<string, string>> facts) {
      return true;
    }

    /// <inheritdoc/>
    public bool Send(byte[] message_id, int type, byte[] message) {
      return true;
    }

    /// <inheritdoc/>
    public bool Send(byte[] message_id, int type, byte[] message,
      byte[] destination) {
      return true;
    }

    /// <inheritdoc/>
    public bool Send(byte[] message_id, int type, byte[] message,
      IEnumerable<KeyValuePair<string, string>> facts) {
      return true;
    }

    /// <inheritdoc/>
    public bool Send(byte[] message_id, int type, byte[] message,
      byte[] destination, IEnumerable<KeyValuePair<string, string>> facts) {
      return true;
    }

    /// <inheritdoc/>
    public bool Send(byte[] message_id, int type, byte[] message, string token) {
      return true;
    }

    /// <inheritdoc/>
    public bool Send(byte[] message_id, int type, byte[] message, string token,
      byte[] destination) {
      return true;
    }

    /// <inheritdoc/>
    public bool Send(byte[] message_id, int type, byte[] message, string token,
      IEnumerable<KeyValuePair<string, string>> facts) {
      return true;
    }

    /// <inheritdoc/>
    public bool Send(byte[] message_id, int type, byte[] message, string token,
      byte[] destination, IEnumerable<KeyValuePair<string, string>> facts) {
      return true;
    }

    IRubyMessageSender IRubyMessageSenderFactory.CreateRubyMessageSender() {
      return new NullMessageSender();
    }
  }
}
