using System;

namespace Nohros.Ruby.Protocol
{
  /// <summary>
  /// An implementation of the <see cref="IRubyMessage"/> interface.
  /// </summary>
  public partial class RubyMessage : IRubyMessage
  {
    byte[] IRubyMessage.Message {
      get { return Message.ToByteArray(); }
    }

    byte[] IRubyMessage.Sender {
      get { return Sender.ToByteArray(); }
    }

    byte[] IRubyMessage.Id {
      get { return Id.ToByteArray(); }
    }
  }
}
