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
  public class CreateServiceFactTableCommand : IQuery<int>
  {
    const string kClassName =
      "Nohros.Ruby.Data.SQLite.CreateServiceFactTableCommand";

    const string kExecute = @"
CREATE TABLE service_fact (
  service_id INTEGER,
  service_fact_hash TEXT
)";

    readonly RubyLogger logger_ = RubyLogger.ForCurrentProcess;
    readonly SQLiteConnection sqlite_connection_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateServiceFactTableCommand"/>
    /// using the specified sql connection provider.
    /// </summary>
    /// <param name="sqlite_connection">
    /// A <see cref="SQLiteConnection"/> object that can be used to
    /// create connections to the data provider.
    /// </param>
    public CreateServiceFactTableCommand(SQLiteConnection sqlite_connection) {
      sqlite_connection_ = sqlite_connection;
      logger_ = RubyLogger.ForCurrentProcess;
    }
    #endregion

    public int Execute() {
      using (var builder = new CommandBuilder(sqlite_connection_)) {
        IDbCommand cmd = builder
          .SetText(kExecute)
          .Build();
        try {
          return cmd.ExecuteNonQuery();
        } catch (SqlException e) {
          logger_.Error(string.Format(R.Log_MethodThrowsException, "Execute",
            kClassName), e);
          throw new ProviderException(e);
        }
      }
    }
  }
}
