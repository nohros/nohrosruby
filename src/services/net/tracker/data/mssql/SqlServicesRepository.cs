using System;
using System.Collections.Generic;
using Nohros.Data.SqlServer;

namespace Nohros.Ruby.Data.Sql
{
  public class SqlServicesRepository : IServicesRepository
  {
    readonly SqlConnectionProvider sql_connection_provider_;

    #region .ctor
    public SqlServicesRepository(SqlConnectionProvider sql_connection_provider) {
      sql_connection_provider_ = sql_connection_provider;
    }
    #endregion

    public IServicesByFactsQuery Query(out IServicesByFactsQuery query) {
      query = new ServicesByFactsQuery(sql_connection_provider_);
      return query;
    }

    public INewServiceCommand Query(out INewServiceCommand command) {
      command = new NewServiceCommand(sql_connection_provider_);
      return command;
    }

    public IRemoveServicesCommand Query(out IRemoveServicesCommand query) {
      query = new RemoveServicesCommand(sql_connection_provider_);
      return query;
    }
  }
}
