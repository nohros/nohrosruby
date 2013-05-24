using System;
using System.Collections.Generic;
using Nohros.Data.SqlServer;

namespace Nohros.Ruby.Logging.Data.Sql
{
  public class SqlServiceRepositoryFactory : IServiceRepositoryFactory
  {
    public IServiceRepository CreateServiceRepository(
      IDictionary<string, string> options) {
      var factory = new SqlConnectionProviderFactory();
      var sql_connection_provider = factory
        .CreateProvider(options) as SqlConnectionProvider;
      return new SqlServiceRepository(sql_connection_provider);
    }
  }
}
