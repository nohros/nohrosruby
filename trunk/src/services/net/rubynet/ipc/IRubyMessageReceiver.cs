using System;

namespace Nohros.Ruby
{
  /// <summary>
  /// A <see cref="IRubyMessageReceiver"/> is the object responsible to receive
  /// messages from the ruby service node.
  /// </summary>
  internal interface IRubyMessageReceiver
  {
    /// <summary>
    /// Gets a message packet from a connected client.
    /// </summary>
    /// <returns>
    /// The <see cref="RubyMessagePacket"/> that was received.
    /// </returns>
    /// <remarks>
    /// If there are no messages available <see cref="GetMessagePacket"/>
    /// shall block until the request can be satisfied.
    /// </remarks>
    RubyMessagePacket GetMessagePacket();
  }
}
