using System;

namespace Nohros.Ruby
{
  /// <summary>
  /// A <see cref="IRubyServiceHost"/> is used to host a ruby service and acts
  /// as a communication channel betwwen a service and the external world.
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
  }
}
