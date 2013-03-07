using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Data.SqlClient;
using Nohros.Data;
using Nohros.Data.Providers;
using R = Nohros.Resources.StringResources;

namespace Nohros.Ruby.Data.SQLite
{
  public class ServicesIDsByFacts : IQuery<IEnumerable<int>, ServiceFacts>
  {
    const string kClassName = "Nohros.Ruby.Data.SQLite.ServicesIDsByFacts";
    const string kExecute = @"
select service_id
from service_fact
where service_fact_hash = @service_fact_hash";

    readonly RubyLogger logger_ = RubyLogger.ForCurrentProcess;
    readonly SQLiteConnection sqlite_connection_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="ServicesIDsByFacts"/>
    /// using the specified sql connection provider.
    /// </summary>
    /// <param name="sqlite_connection">
    /// A <see cref="SQLiteConnection"/> object that can be used to
    /// create connections to the data provider.
    /// </param>
    public ServicesIDsByFacts(SQLiteConnection sqlite_connection) {
      sqlite_connection_ = sqlite_connection;
      logger_ = RubyLogger.ForCurrentProcess;
    }
    #endregion

    public IEnumerable<int> Execute(ServiceFacts criteria) {
      using (var builder = new CommandBuilder(sqlite_connection_)) {
        IEnumerator<KeyValuePair<string, string>> enumerator =
          criteria.GetEnumerator();
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
  }
}
