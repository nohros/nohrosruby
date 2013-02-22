using System;
using System.Collections.Generic;
using Nohros.Concurrent;
using Nohros.Ruby.Protocol;

namespace Nohros.Ruby
{
  /// <summary>
  /// A channel implementation that essentially behaves like "/dev/null". All
  /// calls to Send() will return <c>true</c> although no action is performed.
  /// Added listeners are ignored and the Open method do nothing.
  /// </summary>
  internal class NullMessageChannel : IRubyMessageChannel
  {
    /// <inheritdoc/>
    public void AddListener(IRubyMessageListener listener, IExecutor executor) {
    }

    /// <inheritdoc/>
    public void Open() {
    }

    public bool Send(IRubyMessage message) {
      return true;
    }

    public bool Send(IRubyMessage message,
      IEnumerable<KeyValuePair<string, string>> facts) {
      return true;
    }

    public bool Send(byte[] message_id, int type, byte[] message,
      byte[] destination) {
      return true;
    }

    public bool Send(byte[] message_id, int type, byte[] message,
      byte[] destination, IEnumerable<KeyValuePair<string, string>> facts) {
      return true;
    }

    public bool Send(byte[] message_id, int type, byte[] message,
      byte[] destination, string token) {
      return true;
    }

    public bool Send(byte[] message_id, int type, byte[] message,
      byte[] destination, string token,
      IEnumerable<KeyValuePair<string, string>> facts) {
      return true;
    }

    public bool Send(RubyMessagePacket packet) {
      return true;
    }

    public string Endpoint {
      get { return "inproc://null"; }
    }

    public IRubyMessageChannel CreateRubyMessageChannel() {
      return new NullMessageChannel();
    }
  }
}
