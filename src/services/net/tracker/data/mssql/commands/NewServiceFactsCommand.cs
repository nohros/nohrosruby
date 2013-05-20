using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Nohros.Data;
using Nohros.Data.SqlServer;
using R = Nohros.Resources.StringResources;

namespace Nohros.Ruby.Data.Sql
{
  internal class NewServiceFactsCommand
  {
    const string kClassName = "Nohros.Ruby.Data.SQLite.NewServiceFactsCommand";
    const string kExecute = ".rby_service_fact_add";

    readonly RubyLogger logger_ = RubyLogger.ForCurrentProcess;
    readonly SqlConnectionProvider sql_connection_provider_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="NewServiceFactsCommand"/>
    /// using the specified sql connection provider.
    /// </summary>
    public NewServiceFactsCommand(SqlConnectionProvider sql_connection_provider) {
      sql_connection_provider_ = sql_connection_provider;
      logger_ = RubyLogger.ForCurrentProcess;
    }
    #endregion

    public int Execute() {
      using (SqlConnection conn = sql_connection_provider_.CreateConnection())
      using (var builder = new CommandBuilder(conn)) {
        var parms = new IDbDataParameter[1];
        IDbCommand cmd = builder
          .SetText(kExecute)
          .AddParameter("@service_id", ServiceID)
          .AddParameter("@fact_hash", DbType.String, out parms[0])
          .Build();
        try {
          int affected_rows = 0;
          foreach (KeyValuePair<string, string> fact in Facts) {
            parms[0].Value = ServiceFacts.ComputeHash(fact);
            affected_rows += cmd.ExecuteNonQuery();
          }
          return affected_rows;
        } catch (SqlException e) {
          logger_.Error(string.Format(R.Log_MethodThrowsException, "Execute",
            kClassName), e);
          throw new ProviderException(e);
        }
      }
    }

    public NewServiceFactsCommand SetFacts(ServiceFacts facts)
    {
      Facts = facts;
      return this;
    }

    public NewServiceFactsCommand SetServiceID(int service_id) {
      ServiceID = service_id;
      return this;
    }

    int ServiceID { get; set; }

    ServiceFacts Facts { get; set; }
  }
}
