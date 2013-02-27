using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Data.SqlClient;
using Nohros.Data;
using Nohros.Data.Providers;
using Nohros.Data.Providers.SQLite;
using R = Nohros.Resources.StringResources;

namespace Nohros.Ruby.SQLite
{
  public class ServicesByFactsQuery : IQuery<ZMQEndPoint, ServiceFacts>
  {
    const string kClassName = "Nohros.Ruby.SQLite.ServicesByFactsQuery";
    const string kExecute = @"
select
from services s
  inner join service_facts f on f.service_id = s.service_id
where service_facts_hash = @service_facts_hash
";

    readonly RubyLogger logger_ = RubyLogger.ForCurrentProcess;
    readonly SQLiteConnectionProvider sqlite_connection_provider_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="ServicesByFactsQuery"/>
    /// using the specified sqlite connection provider.
    /// </summary>
    /// <param name="sqlite_connection_provider">
    /// A <see cref="SQLiteConnectionProvider"/> object that can be used to
    /// create connections to the data provider.
    /// </param>
    public ServicesByFactsQuery(SQLiteConnectionProvider sqlite_connection_provider) {
      sqlite_connection_provider_ = sqlite_connection_provider;
      logger_ = RubyLogger.ForCurrentProcess;
    }
    #endregion

    public ZMQEndPoint Execute(ServicesByFactsCriteria criteria) {
      using (SQLiteConnection conn = sqlite_connection_provider_.CreateConnection())
      using (var builder = new CommandBuilder(conn)) {
        IDbCommand cmd = builder
          .SetText(sqlite_connection_provider_.Schema + kExecute)
          .SetType(CommandType.StoredProcedure)
          .AddParameter("@service_facts_hash", criteria.Facts.Hash)
          .Build();
        try {
          conn.Open();
          
        } catch (SqlException e) {
          logger_.Error(string.Format(R.Log_MethodThrowsException, "Execute",
            kClassName), e);
          throw new ProviderException(e);
        }
      }
    }
  }
}
