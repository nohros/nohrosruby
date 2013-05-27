using System;
using System.Collections.Generic;
using Nohros.Data.Json;
using Nohros.Ruby.Logging.Data;
using Service = ServiceStack.ServiceInterface.Service;

namespace Nohros.Ruby.Logging
{
  public class ServicesService : Service
  {
    readonly IRegisteredServicesQuery registered_services_query_;
    readonly IServiceRepository service_repository_;

    #region .ctor
    public ServicesService(IServiceRepository service_repository) {
      service_repository_ = service_repository;
      service_repository_.Query(out registered_services_query_);
    }
    #endregion

    public string Get(ServicesRequest request) {
      try {
        IEnumerable<Data.Service> services =
          registered_services_query_.Execute();
        return new JsonStringBuilder()
          .WriteBeginArray()
          .ForEach(services, (service, builder) => builder
            .WriteBeginObject()
            .WriteMember("name", service.Name)
            .WriteMember("displayName", service.DisplayName)
            .WriteMember("maxIdleTime", service.MaxIdleTime)
            .WriteMember("status", (int) service.Status.Type)
            .WriteEndObject())
          .WriteEndArray()
          .ToString();
      } catch (ProviderException) {
        return "[]";
      }
    }
  }
}
