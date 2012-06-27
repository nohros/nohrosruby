using System;
using Nohros.Concurrent;
using Nohros.Ruby.Protocol;

namespace Nohros.Ruby
{
  /// <summary>
  /// Provides a communication channel that exchanges ruby messages.
  /// </summary>
  public interface IRubyMessageChannel : IRubyMessageSender
  {
    /// <summary>
    /// Adds a listener to receive notifications for incoming messages.
    /// </summary>
    /// <param name="listener">
    /// A <see cref="IRubyMessageListener"/> that wants to receive
    /// notifications for incoming messages.
    /// </param>
    /// <param name="executor">
    /// A <see cref="IExecutor"/> object that is used to execute the
    /// <see cref="IRubyMessageListener.OnMessagePacketReceived"/> callback.
    /// </param>
    /// <param name="service">
    /// The name of the service associated with the listener.
    /// </param>
    /// <remarks>
    /// Each listener should be associated with a service(real or virtual).
    /// It will receive only the messages that is destinated to the
    /// associated service.
    /// </remarks>
    void AddListener(IRubyMessageListener listener, IExecutor executor,
      string service);

    /// <summary>
    /// Sends a message packet to the ruby service node.
    /// </summary>
    /// <param name="packet">
    /// The message packet to send.
    /// </param>
    /// <returns>
    /// <c>true</c> is the message was succesfully sent; otherwise,
    /// <c>false</c>.
    /// </returns>
    bool Send(RubyMessagePacket packet);

    /// <summary>
    /// Opens the communication channel.
    /// </summary>
    /// <remarks>
    /// A <see cref="RubyMessageChannel"/> should be opened before it can receive/send
    /// any messages.
    /// </remarks>
    void Open();
  }
}
