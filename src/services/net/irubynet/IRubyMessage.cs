using System;
using System.Collections.Generic;
using System.Text;

namespace Nohros.Ruby
{
    /// <summary>
    /// Defines the messages that is used in ruby server/client communication.
    /// </summary>
    /// <remarks>
    /// This interface acts as a message metadata. More specifically it provides a convenient way
    /// to dispatch messages to/from the a service.
    /// </remarks>
    public interface IRubyMessage
    {
        /// <summary>
        /// Gets the message header.
        /// </summary>
        /// <remarks>
        /// The message header contains the message metadata. Services rely on the information contained
        /// in header to correctly decode the <see cref="Message"/>
        /// </remarks>
        IRubyMessageHeader Header { get; }

        /// <summary>
        /// Gets the service specific message encoded into protocol buffer format.
        /// </summary>
        string Message { get; }
    }
}
