using System;
using System.Collections.Generic;
using System.Text;

namespace Nohros.Ruby.Service.Net
{
    /// <summary>
    /// This class is used to avoid the return of null when a IRubyServiceHost object is needed. It does
    /// nothing and acts like a mock.
    /// </summary>
    public class EmptyServiceHost : IRubyServiceHost
    {
        IRubyService service_;

        #region .ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="EmptyServiceHost"/> class.
        /// </summary>
        public EmptyServiceHost() {
            service_ = new EmptyService(string.Empty);
        }
        #endregion

        /// <summary>
        /// Sends a message to the ruby server.
        /// </summary>
        /// <returns>true if the message is succesfully send; otherwise, false.</returns>
        /// <remarks>
        /// This service host is fake, so this method alwyas returns false.
        /// </remarks>
        public bool SendMessage(IRubyMessagePacket message) {
            return false;
        }

        /// <summary>
        /// Starts the service.
        /// </summary>
        /// <remarks>This service does nothing so it can not be started.Calling this method does not
        /// produces any effect.</remarks>
        public void StartService() { }

        /// <summary>
        /// Stops the service.
        /// </summary>
        /// <remarks>This service does nothing so it can not be started and cannot be stopped. Calling this
        /// method does not produces any effect.</remarks>
        public void StopService() { }

        /// <summary>
        /// Gets a reference to the running service.
        /// </summary>
        /// <remarks>The runing service is a instance of the <see cref=""/></remarks>
        public IRubyService Service {
            get { return service_; }
        }
    }
}
