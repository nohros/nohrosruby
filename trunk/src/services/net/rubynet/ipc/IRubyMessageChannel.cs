using System;
using Nohros.Concurrent;

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
    /// A <see cref="IExecutor"/> that executes the
    /// <see cref="IRubyMessageListener.OnMessagePacketReceived"/> callback
    /// method.
    /// </param>
    void AddListener(IRubyMessageListener listener, IExecutor executor);

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
