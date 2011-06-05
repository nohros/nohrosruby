using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Nohros.Ruby
{
    /// <summary>
    /// Defines a way to execute code from a .NET assembly library.
    /// </summary>
    public interface IRubyService
    {
        /// <summary>
        /// Gets the name of the service.
        /// </summary>
        /// <remarks>
        /// RubyNet service host has the ability to run more than one service within a single process. Services can
        /// be stopped any time and its name is used to locate and stop it.
        /// <para>The RubyNet service host allows services with same name to run in the same process. </para>
        /// <para>If more than one service with same name is running within a single RubyNet host process a stop
        /// command will stop all this service.</para>
        /// </remarks>
        string Name { get ; }

        /// <summary>
        /// Starts the service.
        /// </summary>
        void Run();

        /// <summary>
        /// Stops the service, shut in it down.
        /// </summary>
        /// <remarks>
        /// A service could be stopped at any time by using a stop command at the command line or by sending
        /// a stop command through the RubyNet service host IPC channel. If the service host receives a stop
        /// command request for a service, it instructs the service to stop by calling this method. The service
        /// name is used to locate the service that must be stopped. All the services that is found with the
        /// given name will be stopped.
        /// <para>
        /// By default, a service has approximately 20 seconds to perform cleanup tasks. After this time expires
        /// the RubyNet host process forces the service shutdown regardless of whether service shutdown is complete.
        /// </para>
        /// <para>
        /// Services should complete their cleanup tasks as quickly as possible. It is good practice to minimize
        /// unsaved data by saving data on a regular basis, keeping track of the data that is saved, and only saving
        /// your unsaved data on shutdown.
        /// </para>
        /// </remarks>
        void Stop();

        /// <summary>
        /// Handle messages sent from ruby server.
        /// </summary>
        /// <param name="message">The message sent from ruby server.</param>
        /// <returns>true if the message was successfully processed; otherwise false.</returns>
        /// <remarks>
        /// The message dispatcher in the main thread of a ruby service process invokes the message handler
        /// function for the specified service whenever it receives a message from the ruby server. After
        /// receiving the message, the message handler function must return true if the message was
        /// succesfully processed or false if not.
        /// <para>
        /// The message handler function is intended to receive messages and return immediately. The callback
        /// function should save its parameters and create other threads to perform additional work. The ruby
        /// service must ensure that such threads have exited before stopping the service. For this reason, it
        /// is very recommended the use of background threads. In particular, a message handler should avoid
        /// operations that might block, such taking a lock, because this could result in a deadlock or cause
        /// the system to stop responding.
        /// </para>
        /// <para>
        /// When the ruby service host sends a message to a service, it waits for the handler function to return
        /// before sending additional messages. The message handler should return as quick as possible, if it does
        /// not return within 30 seconds, the RSH returns an error. If a service must do lengthy processing when the
        /// service is executing the message handler, it should create a secondary thread to perform the lengthy
        /// processing, and then return from the message handler. This prevents the service from tying up the message
        /// dispatcher.
        /// </para>
        /// </remarks>
        bool OnServerMessage(IRubyMessagePacket message);

        /// <summary>
        /// Gets the status information for a service. This property is used by services to report its current
        /// status to the ruby service host.
        /// </summary>
        /// <remarks>
        /// When a service object is instantiated it is not running and is not performing any stopping operation,
        /// so at this time the value of this property must be <see cref="ServiceStatus.Stopped"/>.
        /// <para>
        /// If the service status could not be retrieved by the service process host or if it has a unregognized
        /// value the service will be forced to shutdown.
        /// </para>
        /// </remarks>
        ServiceStatus Status { get; }
    }
}