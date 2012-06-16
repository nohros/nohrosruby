using System;

namespace Nohros.Ruby
{
  /// <summary>
  /// A <see cref="IRubyMessageListener"/> is the object responsible to listen
  /// for messages from a <see cref="IRubyMessageReceiver"/> and delivers
  /// them to all listeners.
  /// </summary>
  public interface IRubyMessageListener
  {
    /// <summary>
    /// Notifies the listener that a message packet was received.
    /// </summary>
    /// <param name="packet">
    /// The message packet that was received.
    /// </param>
    void OnMessagePacketReceived(RubyMessagePacket packet);
  }
}
