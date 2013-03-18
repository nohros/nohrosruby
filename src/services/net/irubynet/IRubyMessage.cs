using System;
using System.Collections.Generic;

namespace Nohros.Ruby.Protocol
{
  /// <summary>
  /// Defines the messages that is used in the ruby server/client
  /// communication.
  /// </summary>
  /// <remarks>
  /// This interface acts as a message metadata. More specifically it
  /// provides a convenient way to dispatch messages to/from the a service.
  /// </remarks>
  public interface IRubyMessage
  {
    /// <summary>
    /// Gets or sets a <see cref="Int64"/> value that is identify the message.
    /// </summary>
    /// <remarks>
    /// The value of this property is can be used to match request/response
    /// messages.
    /// </remarks>
    byte[] Id { get; }

    /// <summary>
    /// Gets a number that identifies the type of message.
    /// </summary>
    /// <remarks>
    /// The meaning of this value is service-dependant. This property provides
    /// a way to identify the message through a integer instead of a string.
    /// A service could use this property as a enumeration, so it could
    /// switch over it instead of use a exprensive string comparison(token).
    /// The value of this field is useful for languages that does not support
    /// reflection or by services that does not want to use reflection.
    /// </remarks>
    int Type { get; }

    /// <summary>
    /// Gets or sets a string that identifies the encoded mesage.
    /// </summary>
    /// <remarks>
    /// The meaning of the value stored into this field is service-dependant.
    /// <para>
    /// Typically this filed is used to store the assembly qualified name of a
    /// specific type or the name of a service method, but it could contain
    /// anything that a service judge meaningful.
    /// </para>
    /// </remarks>
    string Token { get; }

    /// <summary>
    /// Gets a sequence of bytes containing the service specific message
    /// encoded accordingly to the google protobuf format.
    /// </summary>
    /// <remarks>
    /// The meaning of the value stored in this property is service-dependant.
    /// </remarks>
    byte[] Message { get; }

    /// <summary>
    ///A seguence of bytes that identifies the message sender.
    /// </summary>
    /// <remarks>
    /// The message sender value could be set to the sender name, the sender
    /// IP address, the sender UUID or anything else that could be serialized
    /// as a sequence of bytes and identifies the sender.
    /// </remarks>
    byte[] Sender { get; }

    /// <summary>
    /// Serializes a <see cref="IRubyMessage"/> object into a byte array.
    /// </summary>
    byte[] ToByteArray();

    /// <summary>
    /// Contains aditional information about the message.
    /// </summary>
    /// <remarks>
    /// This is used to provide detailed information about the message only if
    /// explicitly required.
    /// </remarks>
    string[] ExtraInfo { get; }
  }
}
