using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Nohros.Data;
using Nohros.Data.SqlServer;
using R = Nohros.Resources.StringResources;

namespace Nohros.Ruby.Data.Sql
{
  internal class NewServiceCommand : INewServiceCommand
  {
    const string kClassName = "Nohros.Ruby.Data.SQLite.NewServiceCommand";
    const string kExecute = @".rby_service_add";

    readonly RubyLogger logger_ = RubyLogger.ForCurrentProcess;
    readonly SqlConnectionProvider sql_connection_provider_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="NewServiceCommand"/>
    /// using the specified sql connection provider.
    /// </summary>
    public NewServiceCommand(SqlConnectionProvider sql_connection_provider) {
      sql_connection_provider_ = sql_connection_provider;
      logger_ = RubyLogger.ForCurrentProcess;
    }
    #endregion

    public int Execute() {
      using (SqlConnection conn = sql_connection_provider_.CreateConnection())
      using (var builder = new CommandBuilder(conn)) {
        IDbCommand cmd = builder
          .SetText(kExecute)
          .AddParameter("@endpoint", Endpoint.Endpoint)
          .Build();
        try {
          int affected = cmd.ExecuteNonQuery();
          return affected + new NewServiceFactsCommand(sql_connection_provider_)
            .SetFacts(Facts)
            .SetServiceID((int) cmd.ExecuteScalar())
            .Execute();
        } catch (SqlException e) {
          logger_.Error(string.Format(R.Log_MethodThrowsException, "Execute",
            kClassName), e);
          throw new ProviderException(e);
        }
      }
    }

    public INewServiceCommand SetFacts(ServiceFacts facts) {
      Facts = facts;
      return this;
    }

    public INewServiceCommand SetEndPoint(ZMQEndPoint endpoint) {
      Endpoint = endpoint;
      return this;
    }

    ServiceFacts Facts { get; set; }
    ZMQEndPoint Endpoint { get; set; }
  }
}
