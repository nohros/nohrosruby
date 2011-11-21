using System;
using System.Collections.Generic;
using System.Text;

namespace Nohros.Ruby
{
  /// <summary>
  /// The message header. Contains information about the packed message.
  /// </summary>n
  public interface IRubyMessageHeader
  {
    /// <summary>
    /// Gets the Id of the packet that is used to match the request/response.
    /// </summary>
    int Id { get; }

    ///<sumary>
    /// Gets the size (in bytes) of the packed message.
    /// </sumary>
    /// <remarks>THe value of that property represents the size of the service
    /// specific message. This information is used by the ruby service host to
    /// correctly pack the message and send it to the server. If this value is
    /// incorrectly defined, the message created will not be fully dispatched
    /// to the server and possibly discarded.
    /// </remarks>
    int Size { get; }

    /// <summary>
    /// A string used to identify the message.
    /// </summary>
    /// <remarks>
    /// The meaning of the value stored into this field is service-dependant.
    /// For example, a service could use this field to store the name of a
    /// message and them use this value to locate a class that could parse the
    /// message or it could store the name of the class that can parse the
    /// message, so it could instantiate the class directly throught reflection
    /// (in languages that support it, of course).
    /// </remarks>
    string MessageType { get; }
  }
}