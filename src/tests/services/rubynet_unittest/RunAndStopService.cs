using System;
using Nohros.Ruby.Protocol;

namespace Nohros.Ruby.Tests
{
  public class RunAndStopService : AbstractRubyService, IRubyServiceFactory
  {
    #region .ctor
    public RunAndStopService()
      : base("RunAndStopService") {
    }
    #endregion

    IRubyService IRubyServiceFactory.CreateService(string command_line) {
      return new RunAndStopService();
    }

    public override IRubyMessage OnMessage(IRubyMessage message) {
      return message;
    }
  }
}
