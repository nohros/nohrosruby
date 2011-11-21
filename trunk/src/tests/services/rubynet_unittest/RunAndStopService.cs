using System;
using System.Collections.Generic;
using System.Text;

using Nohros.Ruby.Service;

namespace Nohros.Ruby.Tests.Net
{
  public class RunAndStopService: IRubyService, IRubyServiceFactory
  {
    string command_line_;

    #region .ctor
    public RunAndStopService() { }
    #endregion

    # region IRubyService
    public void Start() { }

    public void Stop() { }

    public IRubyMessage OnServerMessage(IRubyMessage message) {
      return message;
    }

    public string Name {
      get { return "RunAndStopService"; }
    }
    #endregion

    #region IRubyServiceFactory
    IRubyService IRubyServiceFactory.CreateService(string command_line) {
      return new RunAndStopService();
    }
    #endregion
  }
}
