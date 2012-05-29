using System;

using Nohros.Configuration;

namespace Nohros.Ruby.Weblog
{
  public class WeblogSettings : MustConfiguration
  {
    int publisher_port_ ;

    public WeblogSettings() {
      publisher_port_ = 8523;
    }

    /// <summary>
    /// Gets the port number that is used by the publisher TCP socket.
    /// </summary>
    public int PublisherPort {
      get { return publisher_port_; }
      private set { publisher_port_ = value; }
    }
  }
}
