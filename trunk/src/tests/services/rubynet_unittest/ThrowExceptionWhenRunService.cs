using System;
using Nohros.Ruby.Protocol;

namespace Nohros.Ruby.Tests
{
  public class ThrowExceptionWhenRunService : AbstractRubyService,
                                              IRubyServiceFactory
  {
    #region .ctor
    public ThrowExceptionWhenRunService() : base("ThrowExceptionWhenRunService") {
    }
    #endregion

    IRubyService IRubyServiceFactory.CreateService(string command_line) {
      return new ThrowExceptionWhenRunService();
    }

    public void Start() {
      throw new Exception();
    }

    public override IRubyMessage OnMessage(IRubyMessage message) {
      return message;
    }
  }
}
