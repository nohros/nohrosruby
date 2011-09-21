using System;
using System.Collections.Generic;
using System.Text;

namespace Nohros.Ruby.Service.Net
{
    public class EmptyService : IRubyService
    {
        #region .ctor()
        /// <summary>
        /// Initializes a new instance of the service.
        /// </summary>
        /// <param name="command_line">A string representing the service command line.</param>
        public EmptyService(string command_line) { }
        #endregion

        /// <summary>
        /// Starts the service.
        /// </summary>
        /// <remarks>This service do nothing and this function has no meaning.</remarks>
        public void Run() { }

        /// <summary>
        /// Stops the running service.
        /// </summary>
        /// <remarks>This service do nothing and this function has no meaning.</remarks>
        public void Stop() { }

        /// <summary>
        /// Stops the running service.
        /// </summary>
        /// <remarks>This service do nothing, so this function always returns false.</remarks>
        public bool OnServerMessage(IRubyMessagePacket message) { return false; }

        /// <summary>
        /// Gets the name of the service.
        /// </summary>
        public string Name { get { return "EmptyService"; } }

        /// <summary>
        /// Gets the current service state.
        /// </summary>
        /// <remarks>
        /// This service do nothing, so the service status is always stopped.
        /// </remarks>
        public ServiceStatus Status { get { return ServiceStatus.Stopped; } }
    }
}
