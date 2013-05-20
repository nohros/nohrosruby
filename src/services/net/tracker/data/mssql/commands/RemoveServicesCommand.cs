using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Nohros.Data;
using Nohros.Data.SqlServer;
using R = Nohros.Resources.StringResources;

namespace Nohros.Ruby.Data.Sql
{
  public class RemoveServicesCommand : IRemoveServicesCommand
  {
    const string kClassName = "Nohros.Ruby.Data.SQLite.RemoveServicesCommand";

    const string kExecute = @".rby_service_remove";

    readonly RubyLogger logger_ = RubyLogger.ForCurrentProcess;
    readonly SqlConnectionProvider sql_connection_provider_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="RemoveServicesCommand"/>
    /// using the specified sql connection provider.
    /// </summary>
    public RemoveServicesCommand(SqlConnectionProvider sql_connection_provider) {
      sql_connection_provider_ = sql_connection_provider;
      logger_ = RubyLogger.ForCurrentProcess;
    }
    #endregion

    public IRemoveServicesCommand SetFacts(ServiceFacts facts) {
      Facts = facts;
      return this;
    }

    /// <inheritdoc/>
    public int Execute() {
      using (SqlConnection conn = sql_connection_provider_.CreateConnection())
      using (var builder = new CommandBuilder(conn)) {
        IEnumerable<int> ids = new ServicesIDsByFacts(sql_connection_provider_)
          .SetFacts(Facts)
          .Execute();
        IDbDataParameter parameter;
        var transaction = conn.BeginTransaction();
        IDbCommand cmd = builder
          .SetText(kExecute)
          .AddParameter("@service_id", DbType.Int32, out parameter)
          .SetTransaction(transaction)
          .Build();
        try {
          var removed_services_count = 0;
          foreach (int id in ids) {
            parameter.Value = id;
            cmd.ExecuteScalar();
            removed_services_count++;
          }
          transaction.Commit();
          return removed_services_count;
        } catch (SqlException e) {
          try {
            transaction.Rollback();
          } catch (SqlException ie) {
            logger_.Error(string.Format(R.Log_MethodThrowsException, "Execute",
              kClassName), e);
            throw new ProviderException(ie);
          }
          logger_.Error(string.Format(R.Log_MethodThrowsException, "Execute",
            kClassName), e);
          throw new ProviderException(e);
        }
      }
    }

    ServiceFacts Facts { get; set; }
  }
}
