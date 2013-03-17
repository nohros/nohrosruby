using System;
using System.Collections.Generic;
using Nohros.Data;

namespace Nohros.Ruby.Data
{
  public interface IServicesByFactsQuery : IQuery<IEnumerable<ZMQEndPoint>>
  {
    IServicesByFactsQuery SetFacts(ServiceFacts facts);
  }
}
