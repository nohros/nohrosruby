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
        bool SendMessage(IRubyMessage message);
    }
}
