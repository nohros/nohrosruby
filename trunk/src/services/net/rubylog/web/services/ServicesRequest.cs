using System;
using System.Collections.Generic;
using ServiceStack.ServiceHost;

namespace Nohros.Ruby.Logging
{
  [Route("/services")]
  public class ServicesRequest : IReturn<IEnumerable<Data.Service>>
  {
  }
}
