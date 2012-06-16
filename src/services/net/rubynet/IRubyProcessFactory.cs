using System;

namespace Nohros.Ruby
{
  /// <summary>
  /// <see cref="IRubyProcessFactory"/> is a factory used to build instances
  /// of the <see cref="IRubyProcess"/> class.
  /// </summary>
  internal interface IRubyProcessFactory
  {
    /// <summary>
    /// Creates a new instance of the <see cref="IRubyProcess"/> class using
    /// the specified application settings.
    /// </summary>
    /// <param name="settings">
    /// A <see cref="IRubySettings"/> object containing the application
    /// settings.
    /// </param>
    /// <returns>
    /// The newly created <see cref="IRubyProcess"/> object.
    /// </returns>
    IRubyProcess CreateRubyProcess(IRubySettings settings);
  }
}
