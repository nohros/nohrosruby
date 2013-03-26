using System;
using Nohros.Ruby.Protocol;

namespace Nohros.Ruby
{
  internal class NopService : AbstractRubyService
  {
    public override void Shutdown() {
    }

    public override void OnMessage(IRubyMessage message) {
    }
  }
}
