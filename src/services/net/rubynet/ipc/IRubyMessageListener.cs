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

    /// <summary>
    /// Gets an array of strings that is used to filter the packets that is
    /// delivered to the listener.
    /// </summary>
    /// <remarks>
    /// The filter is based on the service name. The listener receive
    /// notifications only for the packets destinated to the services that is
    /// in the <see cref="Filters"/> array. A <see cref="IRubyMessageListener"/>
    /// that have an empty <see cref="Filters"/> of length zero should shall
    /// receive all incoming messages. A non-empty <see cref="Filters"/> shall
    /// subscribe to all messages destinated to a service that is in the
    /// <see cref="Filters"/> array.
    /// <para>
    /// <see cref="Filters"/> should never return <c>null</c>.
    /// </para>
    /// </remarks>
    string[] Filters { get; }
  }
}
