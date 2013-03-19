using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using Nohros.Data;
using R = Nohros.Resources.StringResources;

namespace Nohros.Ruby.Data.SQLite
{
  internal class NewServiceFactsCommand
  {
    const string kClassName = "Nohros.Ruby.Data.SQLite.NewServiceCommand";
    const string kExecute = @"
insert into service_fact_hash(service_id, service_fact_hash)
values(@service_id, @service_fact_hash);
";

    readonly RubyLogger logger_ = RubyLogger.ForCurrentProcess;
    readonly SQLiteConnection sqlite_connection_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="NewServiceCommand"/>
    /// using the specified sql connection provider.
    /// </summary>
    /// <param name="sqlite_connection">
    /// A <see cref="SQLiteConnection"/> object that can be used to execute
    /// SQL queries.
    /// </param>
    public NewServiceFactsCommand(SQLiteConnection sqlite_connection)
    {
      sqlite_connection_ = sqlite_connection;
      logger_ = RubyLogger.ForCurrentProcess;
    }
    #endregion

    public int Execute() {
      using (var builder = new CommandBuilder(sqlite_connection_)) {
        var parms = new IDbDataParameter[1];
        IDbCommand cmd = builder
          .SetText(kExecute)
          .AddParameter("@service_id", ServiceID)
          .AddParameter("@service_fact_hash", DbType.String, out parms[0])
          .Build();
        try {
          int affected_rows = 0;
          foreach (KeyValuePair<string, string> fact in Facts) {
            parms[0].Value = ServiceFacts.ComputeHash(fact);
            affected_rows += cmd.ExecuteNonQuery();
          }
          return affected_rows;
        } catch (SQLiteException e) {
          logger_.Error(string.Format(R.Log_MethodThrowsException, "Execute",
            kClassName), e);
          throw new ProviderException(e);
        }
      }
    }

    public NewServiceFactsCommand SetFacts(ServiceFacts facts) {
      Facts = facts;
      return this;
    }

    public NewServiceFactsCommand SetServiceID(int service_id) {
      ServiceID = service_id;
      return this;
    }

    ServiceFacts Facts { get; set; }
    int ServiceID { get; set; }
  }
}
