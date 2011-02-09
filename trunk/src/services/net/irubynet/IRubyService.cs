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
        string Name { get ; }

        /// <summary>
        /// Starts the service.
        /// </summary>
        void Run();

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
        bool OnServerMessage(IRubyMessage message);

        /// <summary>
        /// Gets the status information for a service. This property is used by services to report its current
        /// status to the ruby service host.
        /// </summary>
        ServiceStatus Status { get; }
    }
}
