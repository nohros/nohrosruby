using System;

namespace Nohros.Ruby.Logging.Data
{
  public interface IServiceRepository
  {
    IRegisteredServicesQuery Query(out IRegisteredServicesQuery query);
  }
}