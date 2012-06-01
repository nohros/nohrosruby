using System;

namespace Nohros.Ruby
{
  /// <summary>
  /// Defines a way to create <see cref="IRubyService"/> objects inside the
  /// library that implements the <see cref="IRubyService"/> interface.
  /// </summary>
  /// <remarks>
  /// <para>
  /// The <see cref="IRubyServiceFactory"/> interface implies a constructor
  /// with no parameters.
  /// </para>
  /// This constructor is called by the ruby service host(RSH) in order to
  /// create a new instance of the <see cref="IRubyService"/> interface.
  /// </remarks>
  public interface IRubyServiceFactory
  {
    /// <summary>
    /// Creates a instance of the <see cref="IRubyService"/> interface.
    /// </summary>
    /// <param name="command_line_string">
    /// A string representing the command line that was received by the ruby
    /// service host(RSH) as a parameter to the service.</param>
    /// <returns>
    /// An object the implements the <see cref="IRubyService"/> interface.
    /// </returns>
    IRubyService CreateService(string command_line_string);
  }
}