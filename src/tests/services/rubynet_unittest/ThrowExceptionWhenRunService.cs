using System;
using System.Collections.Generic;
using Nohros.Ruby.Protocol;

namespace Nohros.Ruby.Tests
{
  public class ThrowExceptionWhenRunService : AbstractRubyService,
                                              IRubyServiceFactory
  {
    readonly IDictionary<string, string> facts_;

    #region .ctor
    public ThrowExceptionWhenRunService() {
      facts_ = new Dictionary<string, string>
      {{"service-name", "ThrowExceptionWhenRunService"}};
    }
    #endregion

    IRubyService IRubyServiceFactory.CreateService(string command_line) {
      return new ThrowExceptionWhenRunService();
    }

    public override void Start(IRubyServiceHost service_host) {
    }

    public override void Stop(IRubyMessage message) {
    }

    public override void OnMessage(IRubyMessage message) {
    }

    public override IDictionary<string, string> Facts {
      get { return facts_; }
    }
  }
}
