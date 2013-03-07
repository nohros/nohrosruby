using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using Nohros.Data;
using R = Nohros.Resources.StringResources;

namespace Nohros.Ruby.Data.SQLite
{
  internal class AddServiceCommand : IQuery<int, ServiceEndpoint>
  {
    const string kClassName = "Nohros.Ruby.Data.SQLite.AddServiceCommand";
    const string kExecute = "";

    readonly RubyLogger logger_ = RubyLogger.ForCurrentProcess;
    readonly SQLiteConnection sqlite_connection_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="AddServiceCommand"/>
    /// using the specified sql connection provider.
    /// </summary>
    /// <param name="sqlite_connection">
    /// A <see cref="SQLiteConnection"/> object that can be used to execute
    /// SQL queries.
    /// </param>
    public AddServiceCommand(SQLiteConnection sqlite_connection) {
      sqlite_connection_ = sqlite_connection;
      logger_ = RubyLogger.ForCurrentProcess;
    }
    #endregion

    public int Execute(ServiceEndpoint criteria) {
      using (var builder = new CommandBuilder(sqlite_connection_)) {
        var parms = new IDbDataParameter[1];
        IDbCommand cmd = builder
          .SetText(kExecute)
          .AddParameter("@endpoint", criteria.Endpoint.Endpoint)
          .AddParameter("@fact_hash", DbType.String, out parms[0])
          .Build();
        try {
          int affected_rows = 0;
          foreach (KeyValuePair<string, string> fact in criteria.Facts) {
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
  }
}
