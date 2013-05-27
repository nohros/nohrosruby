using System;
using System.Web;
using System.Web.Routing;
using Nohros.Ruby.Logging.Data;
using ServiceStack.Text;
using ServiceStack.WebHost.Endpoints;

namespace Nohros.Ruby.Logging
{
  public class Global : HttpApplication
  {
    class HttpApp : AppHostBase
    {
      readonly StatusManager manager_;
      readonly Settings settings_;

      #region .ctor
      public HttpApp(StatusManager manager, Settings settings)
        : base("Ruby Logging Web Service", typeof (HttpApp).Assembly) {
        manager_ = manager;
        settings_ = settings;
      }
      #endregion

      public override void Configure(Funq.Container container) {
        JsConfig.EmitCamelCaseNames = true;

        var factory = new AppFactory();
        container.Register(ctx => factory.CreateServiceRepository(settings_));
        container.Register(manager_);
      }
    }

    void Application_Start(object sender, EventArgs e) {
      var factory = new AppFactory();
      Settings settings = factory.CreateSettings();
      StatusManager manager = factory.CreateStatusManager();
      factory.CreateApp(manager, settings).Run();
      new HttpApp(manager, settings).Init();
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
