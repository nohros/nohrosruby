using System;

namespace Nohros.Ruby
{
  /// <summary>
  /// A <see cref="IRubyServiceHost"/> is used to host a ruby service and acts
  /// as a communication channel between a service and the external world.
  /// </summary>
  /// <remarks>
  /// A <see cref="IRubyServiceHost"/> provides the common structure used to
  /// host a single <see cref="IRubyService"/>. Basically a
  /// <see cref="IRubyServiceHost"/> manages the service lifecycle, starting,
  /// stoping and handling the communication with the external world.
  /// </remarks>
  public interface IRubyServiceHost: IRubyMessageSender
  {
    /// <summary>
    /// Gets a <see cref="IRubyLogger"/> object that can be used by services
    /// to log messages using the ruby logging infrastructure.
    /// </summary>
    IRubyLogger Logger { get; }

    /// <summary>
    /// Sends a ruby message to the ruby service node informing the receiver
    /// about an error that has been occurred.
    /// </summary>
    /// <param name="error">
    /// A string that describes the error.
    /// </param>
    /// <param name="message_id">
    /// The ID associated with the message that originates the error.
    /// </param>
    /// <param name="exception_code">
    /// A number that provides information about the status of the request.
    /// </param>
    /// <returns>
    /// <c>true</c> is the message was succesfully sent; otherwise,
    /// <c>false</c>.
    /// </returns>
    bool SendError(long message_id, string error, int exception_code);

    /// <summary>
    /// Sends a ruby message to the ruby service node informing the receiver
    /// about an error that has been occurred.
    /// </summary>
    /// <param name="error">
    /// A string that describes the error.
    /// </param>
    /// <param name="exception">
    /// A <see cref="Exception"/> associated with the error.
    /// </param>
    /// <param name="exception_code">
    /// A number that provides information about the status of the request.
    /// </param>
    /// <param name="message_id">
    /// The ID associated with the message that originates the error.
    /// </param>
    /// <returns>
    /// <c>true</c> is the message was succesfully sent; otherwise,
    /// <c>false</c>.
    /// </returns>
    bool SendError(long message_id, string error, int exception_code, Exception exception);

    /// <summary>
    /// Sends a ruby message to the ruby service node informing the receiver
    /// about an error that has been occurred.
    /// </summary>
    /// <param name="exception_code">
    /// A number that provides information about the status of the request.
    /// </param>
    /// <param name="exception">
    /// A <see cref="Exception"/> associated with the error.
    /// </param>
    /// <param name="message_id">
    /// The ID associated with the message that originates the error.
    /// </param>
    /// <returns>
    /// <c>true</c> is the message was succesfully sent; otherwise,
    /// <c>false</c>.
    /// </returns>
    bool SendError(long message_id, int exception_code, Exception exception);
  }
}
