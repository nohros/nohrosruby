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
    /// Sends a ruby message to the service identified by
    /// <paramref name="service"/>.
    /// </summary>
    /// <param name="message">
    /// The message to send.
    /// </param>
    /// <param name="service">
    /// The name of the service to which the message is destinated.
    /// </param>
    /// <returns>
    /// <c>true</c> is the message was succesfully sent; otherwise,
    /// <c>false</c>.
    /// </returns>
    bool Send(IRubyMessage message, string service);
  }
}
