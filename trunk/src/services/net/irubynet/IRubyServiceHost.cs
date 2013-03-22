using System;
using System.Collections.Generic;
using Nohros.Concurrent;

namespace Nohros.Ruby
{
  /// <summary>
  /// A <see cref="IRubyServiceHost"/> is used to host a ruby service and acts
  /// as a communication channel between a service and the external world.
  /// </summary>
  /// <remarks>
  /// A <see cref="IRubyServiceHost"/> provides the common structure used to
  /// host a single <see cref="IRubyService"/>. Basically a
  /// <see cref="IRubyServiceHost"/> manages the service lifecycle, starting,
  /// stoping and handling the communication with the external world.
  /// </remarks>
  public interface IRubyServiceHost : IRubyMessageSender
  {
    /// <summary>
    /// Gets a <see cref="IRubyLogger"/> object that can be used by services
    /// to log messages using the ruby logging infrastructure.
    /// </summary>
    IRubyLogger Logger { get; }

    /// <summary>
    /// Shuts the service host down.
    /// </summary>
    void Shutdown();

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
    /// Announce to the world that the service is running.
    /// </summary>
    /// <param name="facts">
    /// A collection of key/value pairs that can be used to identify a service.
    /// </param>
    /// <remarks>
    /// A service should announce that it is running in order to be dynamically
    /// found by other services. The announcement will contains the facts
    /// declared by the service.
    /// <para>
    /// If a service declares a new fact a new announcment should be performed.
    /// </para>
    /// </remarks>
    void Announce(IDictionary<string, string> facts);
  }
}
