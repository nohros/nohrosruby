using System;
using System.Collections.Generic;
using System.Text;

namespace Nohros.Ruby
{
    /// <summary>
    /// Wraps a protobuf message into a envelope, which defines what is in the message and how to process it.
    /// </summary>
    /// <remarks>
    /// The ruby server send messages that could be one of several different types. However, protocol buffers
    /// parsers cannot necessarily determine the type of a message based on the contents alone. This interface
    /// was created to ensure that the sent message is correctly decoded/encoded by the ruby service.
    /// <para>All messages classes must implements this interface.</para>
    /// </remarks>
    public interface IRubyMessagePacket
    {
        /// <summary>
        /// Gets the varint encoded message representing the command that was sent from the server.
        /// </summary>
        string Message { get; }

        /// <summary>
        /// Gets the fully assembly qualified named of the class that can be used to decode the <see cref="Message"/>.
        /// </summary>
        string ParserType { get; }
    }
}