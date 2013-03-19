using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Data.SqlClient;
using System.Text;
using Nohros.Data;
using Nohros.Data.Providers;
using R = Nohros.Resources.StringResources;

namespace Nohros.Ruby.Data.SQLite
{
  public class ServicesByFactsQuery : IServicesByFactsQuery
  {
    const string kClassName = "Nohros.Ruby.Data.SQLite.ServicesByFactsQuery";

    readonly RubyLogger logger_ = RubyLogger.ForCurrentProcess;
    readonly SQLiteConnection sqlite_connection_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="EventsSinceCriteria"/>
    /// using the specified sql connection provider.
    /// </summary>
    /// <param name="sqlite_connection">
    /// A <see cref="SqlConnectionProvider"/> object that can be used to
    /// create connections to the data provider.
    /// </param>
    public ServicesByFactsQuery(SQLiteConnection sqlite_connection) {
      sqlite_connection_ = sqlite_connection;
      logger_ = RubyLogger.ForCurrentProcess;
    }
    #endregion

    public IServicesByFactsQuery SetFacts(ServiceFacts facts) {
      Facts = facts;
      return this;
    }

    public IEnumerable<ZMQEndPoint> Execute() {
      using (var builder = new CommandBuilder(sqlite_connection_)) {
        IEnumerable<int> ids = new ServicesIDsByFacts(sqlite_connection_)
          .SetFacts(Facts)
          .Execute();
        IDbDataParameter parameter;
        IDbCommand cmd = builder
          .SetText(GetQueryText())
          .AddParameter("@service_id", DbType.Int32, out parameter)
          .Build();
        try {
          var endpoints = new List<ZMQEndPoint>();
          foreach (int id in ids) {
            parameter.Value = id;
            var endpoint = cmd.ExecuteScalar() as string;
            if (endpoint != null) {
              endpoints.Add(new ZMQEndPoint(endpoint));
            }
          }
          return endpoints;
        } catch (SQLiteException e) {
          logger_.Error(string.Format(R.Log_MethodThrowsException,
            "Execute", kClassName), e);
          throw new ProviderException(e);
        }
      }
    }

    string GetQueryText() {
      const string kQueryPrefix = @"
select distinct endpoint
from service s
  inner join service_fact sf on sf.service_id = s.service_id
where service_id = @service_id and service_fact_hash in (";
      var select = new StringBuilder(kQueryPrefix);
      foreach (KeyValuePair<string, string> fact in Facts) {
        select
          .Append("'")
          .Append(ServiceFacts.ComputeHash(fact))
          .Append("',");
      }
      select.Remove(select.Length - 1, 1).Append(")");
      return select.ToString();
    }

    ServiceFacts Facts { get; set; }
  }
}
