using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using Nohros.Data;
using R = Nohros.Resources.StringResources;

namespace Nohros.Ruby.Data.SQLite
{
  internal class NewServiceCommand : INewServiceCommand
  {
    const string kClassName = "Nohros.Ruby.Data.SQLite.NewServiceCommand";
    const string kExecute = @"
insert into service(service_endpoint)
values(@service_endpoint);
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
    public NewServiceCommand(SQLiteConnection sqlite_connection) {
      sqlite_connection_ = sqlite_connection;
      logger_ = RubyLogger.ForCurrentProcess;
    }
    #endregion

    public int Execute() {
      using (var builder = new CommandBuilder(sqlite_connection_)) {
        var parms = new IDbDataParameter[1];
        IDbCommand cmd = builder
          .SetText(kExecute)
          .AddParameter("@service_endpoint", Endpoint.Endpoint)
          .Build();
        try {
          int affected = cmd.ExecuteNonQuery();
          return affected + new NewServiceFactsCommand(sqlite_connection_)
            .SetFacts(Facts)
            .SetServiceID((int) sqlite_connection_.LastInsertRowId)
            .Execute();
        } catch (SQLiteException e) {
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
