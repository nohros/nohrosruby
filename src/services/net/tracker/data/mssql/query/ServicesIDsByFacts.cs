using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Nohros.Data;
using Nohros.Data.SqlServer;
using R = Nohros.Resources.StringResources;

namespace Nohros.Ruby.Data.Sql
{
  public class ServicesIDsByFacts : IQuery<IEnumerable<int>>
  {
    const string kClassName = "Nohros.Ruby.Data.Sql.ServicesIDsByFacts";
    const string kExecute = ".service_fact_get_ids";

    readonly RubyLogger logger_ = RubyLogger.ForCurrentProcess;
    readonly SqlConnectionProvider sql_connection_provider_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="ServicesIDsByFacts"/>
    /// using the specified sql connection provider.
    /// </summary>
    /// <param name="sql_connection_provider">
    /// A <see cref="SqlConnectionProvider"/> object that can be used to
    /// create connections to the data provider.
    /// </param>
    public ServicesIDsByFacts(SqlConnectionProvider sql_connection_provider) {
      sql_connection_provider_ = sql_connection_provider;
      logger_ = RubyLogger.ForCurrentProcess;
    }
    #endregion

    public IEnumerable<int> Execute() {
      using(SqlConnection conn = sql_connection_provider_.CreateConnection())
      using (var builder = new CommandBuilder(conn)) {
        IEnumerator<KeyValuePair<string, string>> enumerator =
          Facts.GetEnumerator();
        if (!enumerator.MoveNext()) {
          return new int[0];
        }

        // get all the services that matches the first fact.
        IDbCommand cmd = builder
          .SetText(kExecute)
          .AddParameter("@service_fact_hash",
            ServiceFacts.ComputeHash(enumerator.Current))
          .Build();
        try {
          var list = new List<int>();
          do {
            using (IDataReader reader = cmd.ExecuteReader()) {
              while (reader.Read()) {
                list.Add(reader.GetInt32(0));
              }
            }
          } while (enumerator.MoveNext());
          return list;
        } catch (SqlException e) {
          logger_.Error(string.Format(R.Log_MethodThrowsException, "Execute",
            kClassName, e));
          throw new ProviderException(e);
        }
      }
    }

    public ServicesIDsByFacts SetFacts(ServiceFacts facts) {
      Facts = facts;
      return this;
    }

    ServiceFacts Facts { get; set; }
  }
}
