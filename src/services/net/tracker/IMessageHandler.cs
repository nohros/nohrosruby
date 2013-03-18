using System;
using Nohros.Ruby.Protocol;

namespace Nohros.Ruby
{
  internal interface IMessageHandler
  {
    void Handle(IRubyMessage message);
  }
}
