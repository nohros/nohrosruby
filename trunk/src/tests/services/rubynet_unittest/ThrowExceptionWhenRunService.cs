using System;
using System.Collections.Generic;
using System.Text;

namespace Nohros.Ruby.Tests.Net
{
  public class ThrowExceptionWhenRunService: IRubyService, IRubyServiceFactory
  {
    #region .ctor
    public ThrowExceptionWhenRunService() { }
    #endregion

    public void Start() {
      throw new Exception();
    }

    public void Stop() {
    }

    public IRubyMessage OnServerMessage(IRubyMessage message) {
      return message;
    }

    public string Name {
      get { return "ThrowExceptionWhenRunService"; }
    }

    IRubyService IRubyServiceFactory.CreateService(string command_line) {
      return new ThrowExceptionWhenRunService();
    }
  }
}