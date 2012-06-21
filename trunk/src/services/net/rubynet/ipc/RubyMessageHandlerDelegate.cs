using System;
using Nohros.Ruby.Protocol;

namespace Nohros.Ruby
{
  /// <summary>
  /// The method that is called when a message is received from a client.
  /// </summary>
  /// <param name="packet">
  /// The received message packet.
  /// </param>
  public delegate void RubyMessageHandlerDelegate(RubyMessagePacket packet);
}
