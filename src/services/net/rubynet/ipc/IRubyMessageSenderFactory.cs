using System;

namespace Nohros.Ruby
{
  /// <summary>
  /// Class used to created instances of the <see cref="IRubyMessageSender"/>
  /// class.
  /// </summary>
  public interface IRubyMessageSenderFactory
  {
    /// <summary>
    /// Creates a new instance of the <see cref="IRubyMessageSender"/> class.
    /// </summary>
    /// <returns>
    /// The newly created <see cref="IRubyMessageSender"/> object.
    /// </returns>
    IRubyMessageSender CreateRubyMessageSender();
  }
}
