using System;
using System.Collections.Generic;
using Nohros.Ruby.Logging.Data;
using Service = ServiceStack.ServiceInterface.Service;

namespace Nohros.Ruby.Logging
{
  public class ServicesService : Service
  {
    readonly IServiceRepository service_repository_;
    readonly IRegisteredServicesQuery registered_services_query_;

    #region .ctor
    public ServicesService(IServiceRepository service_repository) {
      service_repository_ = service_repository;
      service_repository_.Query(out registered_services_query_);
    }
    #endregion

    public IEnumerable<Data.Service> Get(ServicesRequest request) {
      try {
        return registered_services_query_.Execute();
      } catch(ProviderException) {
        return new Data.Service[0];
      }
    }
  }
}
