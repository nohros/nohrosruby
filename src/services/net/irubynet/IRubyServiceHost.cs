using System;

namespace Nohros.Ruby
{
  /// <summary>
  /// A <see cref="IRubyServiceHost"/> is used to host a ruby service.
  /// </summary>
  /// <remarks>
  /// A <see cref="IRubyServiceHost"/> provides the common structure used to
  /// host a single <see cref="IRubyService"/>. Basically a
  /// <see cref="IRubyServiceHost"/> manages the service lifecycle, starting,
  /// stoping and handling the communication with the external world.
  /// </remarks>
  public interface IRubyServiceHost
  {
  }
}
