using System;
using System.Collections.Generic;

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

    string[] IRubyMessage.ExtraInfo {
      get {
        string[] array = new string[ExtraInfoCount];
        ExtraInfoList.CopyTo(array, 0);
        return array;
      }
    }
  }
}
