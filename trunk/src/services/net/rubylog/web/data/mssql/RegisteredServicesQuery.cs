using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Nohros.Data;
using Nohros.Data.SqlServer;
using Nohros.Extensions;
using Nohros.Resources;

namespace Nohros.Ruby.Logging.Data.Sql
{
  public class RegisteredServicesQuery : IRegisteredServicesQuery
  {
    const string kClassName =
      "Nohros.Ruby.Logging.Data.Sql.RegisteredServicesQuery";

    const string kExecute = @"
select service_name
  ,service_display_name
  ,service_max_idle_time
from rb_logging_service
";

    readonly LogLogger logger_ = LogLogger.ForCurrentProcess;
    readonly IDataReaderMapper<Service> mapper_;
    readonly SqlConnectionProvider sql_connection_provider_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="RegisteredServicesQuery"/>
    /// using the specified sql connection provider.
    /// </summary>
    /// <param name="sql_connection_provider">
    /// A <see cref="SqlConnectionProvider"/> object that can be used to
    /// create connections to the data provider.
    /// </param>
    public RegisteredServicesQuery(SqlConnectionProvider sql_connection_provider) {
      sql_connection_provider_ = sql_connection_provider;
      mapper_ = CreateMapper();
      logger_ = LogLogger.ForCurrentProcess;
    }
    #endregion

    public IEnumerable<Service> Execute() {
      using (SqlConnection conn = sql_connection_provider_.CreateConnection())
      using (var builder = new CommandBuilder(conn)) {
        IDbCommand cmd = builder
          .SetText(kExecute)
          .SetType(CommandType.Text)
          .Build();
        try {
          conn.Open();
          using (IDataReader reader = cmd.ExecuteReader()) {
            return mapper_.Map(reader, false);
          }
        } catch (SqlException e) {
          logger_.Error(
            StringResources.Log_MethodThrowsException.Fmt("Execute", kClassName),
            e);
          throw new ProviderException(e);
        }
      }
    }

    public IDataReaderMapper<Service> CreateMapper() {
      return new DataReaderMapperBuilder<Service>(kClassName)
        .Map(x => x.Name, "service_name")
        .Map(x => x.DisplayName, "service_display_name")
        .Map(x => x.MaxIdleTime, "service_max_idle_time")
        .SetFactory(() => new Service {
          Status = Status.Unknown
        })
        .Build();
    }
  }
}
