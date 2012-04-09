using System;
using System.Collections.Generic;
using System.Text;

namespace Nohros.Ruby.Service.Net
{
    public enum CommandType
    {
        /// <summary>
        /// An command that is not recognized.
        /// </summary>
        Unknown,

        /// <summary>
        /// Command used to starts a service.
        /// </summary>
        Start,
        
        /// <summary>
        /// Command used to stops a service.
        /// </summary>
        Stop,

        /// <summary>
        /// Command used to send a message to a service.
        /// </summary>
        Send,

        /// <summary>
        /// Command used to close the command line language interpreter(shell).
        /// </summary>
        Exit,

        /// <summary>
        /// Command used to clear the screen.
        /// </summary>
        Clear,

        /// <summary>
        /// Shows the help text of a given command.
        /// </summary>
        Help
    }
}
