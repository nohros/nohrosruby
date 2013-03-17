using System;
using Nohros.Data;

namespace Nohros.Ruby.Data
{
  public interface IRemoveServicesCommand : IQuery<int>
  {
    IRemoveServicesCommand SetFacts(ServiceFacts facts);
  }
}
