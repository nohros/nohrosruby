using System;

namespace Nohros.Ruby
{
  /// <summary>
  /// A class used to create instances of the <see cref="IRubyMessageChannel"/>
  /// class.
  /// </summary>
  internal interface IRubyMessageChannelFactory
  {
    /// <summary>
    /// Creates a new instance of the <see cref="IRubyMessageChannel"/>
    /// class.
    /// </summary>
    /// <returns>
    /// The newly created <see cref="IRubyMessageChannel"/> object.
    /// </returns>
    IRubyMessageChannel CreateRubyMessageChannel();
  }
}
