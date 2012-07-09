using System;
using Nohros.Ruby.Protocol;

namespace Nohros.Ruby
{
  /// <summary>
  /// A <see cref="IRubyMessageSender"/> is an object that sends messages
  /// through an communication channel.
  /// </summary>
  public interface IRubyMessageSender
  {
    /// <summary>
    /// Sends a ruby message to the ruby service node.
    /// </summary>
    /// <param name="message">
    /// The message to send.
    /// </param>
    /// <returns>
    /// <c>true</c> is the message was succesfully sent; otherwise,
    /// <c>false</c>.
    /// </returns>
    /// <remarks>
    /// This method is used to send a message to the service node that is
    /// hosting a service. It is usually used to send a reply to a messages
    /// that was successfully received and processed.
    /// <para>Ruby does not impose a
    /// strict request/reply message pattern. A received message could have
    /// or not a reply, the service that is receiving the message and the
    /// sender should establish a message pattern that is most suitable for its
    /// operation.
    /// </para>
    /// </remarks>
    bool Send(IRubyMessage message);
  }
}
