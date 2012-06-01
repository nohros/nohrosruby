using System;

namespace Nohros.Ruby
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
    long Id { get; }

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
  }
}
