using System;
using ServiceStack.ServiceHost;

namespace Nohros.Ruby.Logging
{
  [Route("/services/status/")]
  public class StatusRequest
  {
    public string ServiceName { get; set; }
    public long MaxIdleTime { get; set; }
  }
}