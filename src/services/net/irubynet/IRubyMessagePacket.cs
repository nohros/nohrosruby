using System;
using System.Collections.Generic;
using System.Text;

namespace Nohros.Ruby
{
    /// <summary>
    /// Wraps a protobuf message into a envelope, which defines what is in the message and how to
    /// process it.
    /// </summary>
    /// <remarks>
    /// The ruby server send messages that could be one of several different types. However, protocol
    /// buffers parsers cannot necessarily determine the type of a message based on the contents alone.
    /// This interface was created to ensure that the sent message is correctly decoded/encoded by the
    /// ruby service.
    /// <para>All messages classes must implements this interface.</para>
    /// </remarks>
    public interface IRubyMessagePacket
    {
        /// <summary>
        /// Gets the total size of the packet(in bytes), not including the size of this field.
        /// </summary>
        int Size { get; }

        /// <summary>
        /// Gets the Id of the packet that is used to match the request/response.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Gets the name of the service which this message is related.
        /// </summary>
        string ServiceName { get; }

        /// <summary>
        /// A string used to identify the message.
        /// </summary>
        /// <remarks>
        /// The meaning of the value stored into this field is service-dependant. For example,
        /// a service could use this field to store the name of a message and them use this value
        /// to locate a class that could parse the message or it could store the name of the class
        /// that can parse the message, so it could instantiate the class directly throught
        /// reflection(in languages that support it, of course).
        /// </remarks>
        string MessageType { get; }

        /// <summary>
        /// Gets the protocol buffer encoded message that must be dispatched to the service.
        /// </summary>
        /// <remarks>
        /// The meaning of the value of this member is service-dependant. The RSH do not touch this member.
        /// </remarks>
        string Message { get; }
    }
}