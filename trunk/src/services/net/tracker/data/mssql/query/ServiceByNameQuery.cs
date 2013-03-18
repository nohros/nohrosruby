using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Nohros.Data;
using Nohros.Data.Providers;
using Nohros.Resources;

namespace Nohros.Ruby.data.query
{
  public class ServiceByNameQuery :
    IQuery<ServiceMetadata, ServiceByNameCriteria>
  {
    const string kClassName = "Nohros.Ruby.data.query.ServiceByNameQuery";
    const string kExecute = "";

    readonly TrackerLogger logger_ = TrackerLogger.ForCurrentProcess;
    readonly SqlConnectionProvider sql_connection_provider_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceByNameQuery"/>
    /// using the specified sql connection provider.
    /// </summary>
    /// <param name="sql_connection_provider">
    /// A <see cref="SqlConnectionProvider"/> object that can be used to
    /// create connections to the data provider.
    /// </param>
    public ServiceByNameQuery(SqlConnectionProvider sql_connection_provider) {
      sql_connection_provider_ = sql_connection_provider;
      logger_ = TrackerLogger.ForCurrentProcess;
    }
    #endregion

    public ServiceMetadata Execute(ServiceByNameCriteria criteria) {
      using (SqlConnection conn = sql_connection_provider_.CreateConnection())
      using (var builder = new CommandBuilder(conn)) {
        IDbCommand cmd = builder
          .SetText(sql_connection_provider_.Schema + kExecute)
          .SetType(CommandType.StoredProcedure)
          .AddParameter("@service_name", criteria.Name)
          .Build();
        try {
          conn.Open();
          using (IDataReader reader = cmd.ExecuteReader()) {
            if (reader.Read()) {
              return Map(reader);
            }
          }
        } catch (SqlException e) {
          logger_.Error(string.Format(StringResources.Log_MethodThrowsException,
            "Execute", kClassName), e);
        }
      }
    }

    public ServiceMetadata Map(IDataReader reader) {
      return Mappers.GetMapper<ServiceMetadata>(reader,
        () => new[] {
        });
    }
  }
}
