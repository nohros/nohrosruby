using System;
using System.Collections.Generic;
using System.Text;

namespace Nohros.Ruby
{
    /// <summary>
    /// Defines a enumeration of possible service status. A service use this enumeration to report
    /// its current status to the ruby service host.
    /// </summary>
    public enum ServiceStatus
    {
        /// <summary>
        /// The service is running.
        /// </summary>
        Running = 1,

        /// <summary>
        /// The service is stopped.
        /// </summary>
        Stopped = 2,

        /// <summary>
        /// The service is not running.
        /// </summary>
        Stopping = 3
    }
}