using System;
using System.Web;
using System.Web.Routing;
using ServiceStack.WebHost.Endpoints;

namespace Nohros.Ruby.Logging
{
  public class Global : HttpApplication
  {
    class HttpApp : AppHostBase
    {
      readonly StatusManager manager_;

      #region .ctor
      public HttpApp(StatusManager manager)
        : base("Ruby Logging Web Service", typeof (HttpApp).Assembly) {
        manager_ = manager;
      }
      #endregion

      public override void Configure(Funq.Container container) {
        container.Register(manager_);
      }
    }

    void Application_Start(object sender, EventArgs e) {
      var factory = new AppFactory();
      Settings settings = factory.CreateSettings();
      StatusManager manager = factory.CreateStatusManager();
      factory.CreateApp(manager, settings).Run();
      new HttpApp(manager).Init();
      RouteTable.Routes.MapConnection<HttpPublisher>("logs", "/logs");
    }

    void Application_End(object sender, EventArgs e) {
    }

    void Application_Error(object sender, EventArgs e) {
    }

    void Session_Start(object sender, EventArgs e) {
    }

    void Session_End(object sender, EventArgs e) {
    }
  }
}
