using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nohros.Data.Providers;

namespace Nohros.Ruby.Data.Sql
{
  public class SqlServicesRepositoryFactory : IServicesRepositoryFactory
  {
    public IServicesRepository CreateServicesRepository(
      IDictionary<string, string> options) {
      var factory = new SqlConnectionProviderFactory();
      var sql_connection_provider = factory
        .CreateProvider(options) as SqlConnectionProvider;
      return new SqlServicesRepository(sql_connection_provider);
    }
  }
}
