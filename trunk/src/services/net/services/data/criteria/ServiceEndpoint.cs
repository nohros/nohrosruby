using System;

namespace Nohros.Ruby.Data
{
  public class ServiceEndpoint
  {
    public ServiceFacts Facts { get; set; }
    public ZMQEndPoint Endpoint { get; set; }
  }
}
