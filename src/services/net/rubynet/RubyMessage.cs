using System;

namespace Nohros.Ruby
{
  /// <summary>
  /// An implementation of the <see cref="IRubyMessage"/> interface.
  /// </summary>
  public partial class RubyMessage : IRubyMessage
  {
    byte[] IRubyMessage.Message { get { return Message.ToByteArray(); } }
  }
}
