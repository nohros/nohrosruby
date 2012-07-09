using System;
using System.Collections.Generic;
using Nohros.Ruby.Protocol;

namespace Nohros.Ruby
{
  /// <summary>
  /// Defines a way to execute code from a .NET assembly.
  /// <para>
  /// A instance of the <see cref="IRubyService"/> object will be create
  /// throught a specific <see cref="IRubyServiceFactory"/> object.
  /// </para>
  /// </summary>
  /// <seealso cref="IRubyServiceFactory"/>
  public interface IRubyService
  {
    /// <summary>
    /// Gets the name of the service.
    /// </summary>
    /// <remarks>
    /// Ruby service host(RSH) has the ability to run more than one service
    /// within a single process. Services can be stopped any time and its name
    /// is used to locate it.
    /// <para>
    /// The RSH allows services with equals name to run in the same
    /// process.
    /// </para>
    /// <para>
    /// If more than one service with equals name is running within a single
    /// host process a stop command will stop all the services that has the
    /// specified name at once.
    /// </para>
    /// </remarks>
    //string Name { get; }

    /// <summary>
    /// Gets a collection of key/value pairs representing a discreet bits of
    /// information about the service. Examples could be service name,
    /// host name, release, operating system, etc.
    /// </summary>
    /// <para>
    /// The <see cref="Facts"/> is used by the service node to filter the
    /// received messages. Service node guarantee that a service receive only
    /// messages associated with one or more service facts.
    /// <para>
    /// For example, a service that declares the facts
    /// <para>
    /// {"service-name"="service", "host-name"="my.host.name"}
    /// </para>  will receive the messages that has the facts:
    /// <para>
    /// </para>
    /// {"service-name"="service"}
    /// {"host-name"="my.host.name"}
    /// {"service-name"="service", "host-name"="my.host.name"}
    /// </para>, but will not receive messages that has the facts:
    /// <para>
    /// {"service-name"="service", "host-name"="my.host.name", "other-fact":""}
    /// </para>
    /// </para>
    IDictionary<string, string> Facts { get; }

    /// <summary>
    /// Starts the service.
    /// </summary>
    /// <param name="service_host">
    /// A <see cref="IRubyServiceHost"/> object associated with the service
    /// beign created.
    /// </param>
    void Start(IRubyServiceHost service_host);

    /// <summary>
    /// Shuts the service down.
    /// </summary>
    /// <remarks>
    /// A service could be shutted down at any time, but usually it happens
    /// when the machine that is running the service is shutting down.
    /// <para>
    /// By default, a service has approximately 20 seconds to perform cleanup
    /// tasks. After this time expires the RSH forces the service shutdown
    /// regardless of whether service shutdown is complete.
    /// </para>
    /// <para>
    /// Services should complete their cleanup tasks as quickly as possible. It
    /// is good practice to minimize unsaved data by saving data on a regular
    /// basis, keeping track of the data that is saved, and only saving your
    /// unsaved data on shutdown.
    /// </para>
    /// <para>
    /// <see cref="Shutdown"/> is used only to shut the service down, not for
    /// stop the service.
    /// </para>
    /// </remarks>
    void Shutdown();

    /// <summary>
    /// Instructs the service to pause the work that it is doing.
    /// </summary>
    /// <param name="message">
    /// The pause message. The service should use it to check if the pause
    /// request is associated with the executing instance.
    /// </param>
    /// <remarks>
    /// If a service could not be paused it should do nothing.
    /// <para>
    /// <see cref="Pause"/> a service can conserve system resources because
    /// <see cref="Pause"/> need not to release all system resources. For
    /// example, if threads have been opened by the service, pausing it rather
    /// than stopping it can allow threads to remain open, obviating the need
    /// to reallocate then when the service continues.
    /// </para>
    /// </remarks>
    void Pause(IRubyMessage message);

    /// <summary>
    /// Instructs the service to continue the work that it was doing when
    /// it was paused.
    /// </summary>
    /// <param name="message">
    /// The continue message. The service should use it to check if the continue
    /// request is associated with the executing instance.
    /// </param>
    /// <remarks>
    /// <see cref="Continue"/> mirrors the service's response to
    /// <see cref="Pause"/>. When a service is continued, the work that it was
    /// doing when it was paused should becomes active again.
    /// </remarks>
    void Continue(IRubyMessage message);

    /// <summary>
    /// Stops the executing service.
    /// </summary>
    /// <param name="message">
    /// The stop message. The service should use it to check if the stop
    /// request is associated with the executing instance.
    /// </param>
    void Stop(IRubyMessage message);

    /// <summary>
    /// Handle messages sent from clients.
    /// </summary>
    /// <param name="message">
    /// The message sent from a client.
    /// </param>
    /// <returns>
    /// <c>true</c> if the message was can be processed by the running
    /// instance; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// The message dispatcher in the main thread of the ruby service process
    /// invokes the service message handler function whenever it receives a
    /// message from a client directed to a service.
    /// <para>
    /// When the ruby service host sends a message to a service, it waits for
    /// the handler function to return before sending additional messages. The
    /// ruby service host queue up messages that is sent to service while it
    /// is processing another message. When a queue is full, RSH automatically
    /// throws away messages.
    /// </para>
    /// <para>
    /// The message handler should return as soon as possible to avoid losing
    /// messages because the receiving queue is full.
    /// </para>
    /// <para>
    /// The ruby service host (RSH) sends all the messages that is associated
    /// with the service name. The service should check if the received
    /// message should be processed by the running instance and sent back a
    /// response indicating if the message can be processed or not.
    /// </para>
    /// </remarks>
    void OnMessage(IRubyMessage message);
  }
}