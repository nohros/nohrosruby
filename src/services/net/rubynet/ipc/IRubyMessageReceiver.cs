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

    /// <summary>
    /// Establish a message filter on the receiver.
    /// </summary>
    /// <param name="filter">
    /// The filter to establish.
    /// </param>
    /// <remarks>
    /// The filter is based on the service name.
    /// <para>
    /// The newly created <see cref="IRubyMessageReceiver"/> shall filter out
    /// all incoming messages destinated to the service named
    /// <paramref name="filter"/>, therefore you call <see cref="AddFilter"/>
    /// to establish an initial message filter. A empty
    /// <paramref name="filter"/> of length zero shall not filter any incoming
    /// messages. A non-empty <paramref name="filter"/> shall filter all
    /// messages beginnig with destinated to the service named
    /// <paramref name="filter"/>. Multiple filters may be attached to a single
    /// receiver, in which case a message shall be accepted if it matches at
    /// least one filter.
    /// </para>
    /// </remarks>
    void AddFilter(string filter);
  }
}
