using System;
using Nohros.Ruby.Protocol;

namespace Nohros.Ruby
{
  /// <summary>
  /// A <see cref="IRubyMessageListener"/> is the object responsible to listen
  /// for messages from a node and delivers them to all listeners.
  /// </summary>
  public interface IRubyMessageListener
  {
    /// <summary>
    /// Notifies the listener that a message packet was received.
    /// </summary>
    /// <param name="packet">
    /// The message packet that was received.
    /// </param>
    void OnMessageReceived(IRubyMessage packet);
  }
}
