using System;
using System.Web;
using System.Web.Routing;

namespace Nohros.Ruby.Logging
{
  public class Global : HttpApplication
  {
    void Application_Start(object sender, EventArgs e) {
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
