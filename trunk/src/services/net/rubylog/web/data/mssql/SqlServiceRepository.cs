using System;
using Nohros.Data.SqlServer;

namespace Nohros.Ruby.Logging.Data.Sql
{
  public class SqlServiceRepository : IServiceRepository
  {
    readonly SqlConnectionProvider sql_connection_provider_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlConnectionProvider"/>
    /// class by using the specified <paramref name="sql_connection_provider"/>.
    /// </summary>
    /// <param name="sql_connection_provider">
    /// A <see cref="SqlConnectionProvider"/>
    /// </param>
    public SqlServiceRepository(SqlConnectionProvider sql_connection_provider) {
      sql_connection_provider_ = sql_connection_provider;
    }
    #endregion

    /// <inheritdoc/>
    public IRegisteredServicesQuery Query(out IRegisteredServicesQuery query) {
      return query = new RegisteredServicesQuery(sql_connection_provider_);
    }
  }
}
