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
  public class CreateServiceTableCommand : IQuery<int, NewServiceTableCriteria>
  {
    const string kClassName =
      "Nohros.Ruby.Data.SQLite.CreateServiceTableCommand";

    const string kExecute = @"
CREATE TABLE IF NOT EXISTS service (
  service_id INTEGER PRIMARY KEY,
  service_endpoint TEXT
)";

    readonly RubyLogger logger_ = RubyLogger.ForCurrentProcess;
    readonly SQLiteConnection sqlite_connection_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateServiceTableCommand"/>
    /// using the specified sql connection provider.
    /// </summary>
    /// <param name="sqlite_connection">
    /// A <see cref="SQLiteConnection"/> object that can be used to
    /// create connections to the data provider.
    /// </param>
    public CreateServiceTableCommand(SQLiteConnection sqlite_connection) {
      sqlite_connection_ = sqlite_connection;
      logger_ = RubyLogger.ForCurrentProcess;
    }
    #endregion

    public int Execute(NewServiceTableCriteria criteria) {
      using (var builder = new CommandBuilder(sqlite_connection_)) {
        IDbCommand cmd = builder
          .SetText(kExecute)
          .Build();
        try {
          return cmd.ExecuteNonQuery();
        } catch (SQLiteException e) {
          logger_.Error(string.Format(R.Log_MethodThrowsException, "Execute",
            kClassName), e);
          throw new ProviderException(e);
        }
      }
    }
  }
}
