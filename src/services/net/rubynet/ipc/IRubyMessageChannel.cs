using System;
using Nohros.Concurrent;
using Nohros.Ruby.Protocol;

namespace Nohros.Ruby
{
  /// <summary>
  /// Provides a communication channel that exchanges ruby messages.
  /// </summary>
  internal interface IRubyMessageChannel : IRubyMessageSender
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
    /// <remarks>
    /// Each listener should be associated with a service(real or virtual).
    /// It will receive only the messages that is destinated to the
    /// associated service.
    /// </remarks>
    void AddListener(IRubyMessageListener listener, IExecutor executor);

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
    /// A <see cref="RubyMessageChannel"/> should be opened before it can
    /// receive/send any messages.
    /// </remarks>
    void Open();

    /// <summary>
    /// Gets the channel's endpoint.
    /// </summary>
    /// <remarks>
    /// The channel endpoint is a string consisting of two parts as follows:
    /// <para>
    /// address:port
    /// </para>
    /// <para>
    /// The address part is the IP address or host name of the channel and
    /// the port is the port number of the channel.
    /// </para>
    /// </remarks>
    string Endpoint { get; }
  }
}
