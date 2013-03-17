using System;
using Nohros.Data;

namespace Nohros.Ruby.Data
{
  public interface INewServiceCommand : IQuery<int>
  {
    INewServiceCommand SetFacts(ServiceFacts facts);
    INewServiceCommand SetEndPoint(ZMQEndPoint endpoint);
  }
}
