using System;
using System.Collections.Generic;
using System.Text;

namespace Nohros.Ruby
{
    /// <summary>
    /// Defines methods and properties for a ruby service host.
    /// </summary>
    public interface IRubyServiceHost
    {
        /// <summary>
        /// Sends a message to the ruby server.
        /// </summary>
        /// <returns>true if the message is succesfully send; otherwise, false.</returns>
        bool SendMessage(IRubyMessagePacket message);

        /// <summary>
        /// Starts the hosted service.
        /// </summary>
        void StartService();

        /// <summary>
        /// Stops the hosted service.
        /// </summary>
        void StopService();

        /// <summary>
        /// Gets a reference to the running service.
        /// </summary>
        IRubyService Service { get; }
    }
}
