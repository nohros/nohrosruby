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
    public void AddListener(IRubyMessagePacketListener packet_listener, IExecutor executor) {
    }

    /// <inheritdoc/>
    public void Open() {
    }

    public void Close() {
    }

    public void Close(int timeout) {
    }

    public bool Send(IRubyMessage message) {
      return true;
    }

    public bool Send(IRubyMessage message,
      IEnumerable<KeyValuePair<string, string>> facts) {
      return true;
    }

    public bool Send(byte[] message_id, int type, byte[] message) {
      return true;
    }

    public bool Send(byte[] message_id, int type, byte[] message,
      byte[] destination) {
      return true;
    }

    public bool Send(byte[] message_id, int type, byte[] message,
      IEnumerable<KeyValuePair<string, string>> facts) {
      return true;
    }

    public bool Send(byte[] message_id, int type, byte[] message,
      byte[] destination, IEnumerable<KeyValuePair<string, string>> facts) {
      return true;
    }

    public bool Send(byte[] message_id, int type, byte[] message, string token) {
      return true;
    }

    public bool Send(byte[] message_id, int type, byte[] message, string token,
      byte[] destination) {
      return true;
    }

    public bool Send(byte[] message_id, int type, byte[] message, string token,
      IEnumerable<KeyValuePair<string, string>> facts) {
      return true;
    }

    public bool Send(byte[] message_id, int type, byte[] message, string token,
      byte[] destination, IEnumerable<KeyValuePair<string, string>> facts) {
      return true;
    }

    public bool Send(RubyMessagePacket packet) {
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

    public IRubyMessageChannel CreateRubyMessageChannel() {
      return new NullMessageChannel();
    }

    public string Endpoint {
      get { return "inproc://null"; }
    }
  }
}
