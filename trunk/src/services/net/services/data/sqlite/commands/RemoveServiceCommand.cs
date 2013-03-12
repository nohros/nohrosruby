using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using Nohros.Data;
using R = Nohros.Resources.StringResources;

namespace Nohros.Ruby.Data.SQLite
{
  public class RemoveServiceCommand : IQuery<int, ServiceFacts>
  {
    const string kClassName = "Nohros.Ruby.Data.SQLite.RemoveServiceCommand";

    const string kExecute = @"
delete from service
where service_id = @service_id;
delete from service_fact
where service_id = @service_id";

    readonly RubyLogger logger_ = RubyLogger.ForCurrentProcess;
    readonly SQLiteConnection sqlite_connection_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="RemoveServiceCommand"/>
    /// using the specified sql connection provider.
    /// </summary>
    /// <param name="sqlite_connection">
    /// A <see cref="SQLiteConnection"/> object that can be used to
    /// create connections to the data provider.
    /// </param>
    public RemoveServiceCommand(SQLiteConnection sqlite_connection) {
      sqlite_connection_ = sqlite_connection;
      logger_ = RubyLogger.ForCurrentProcess;
    }
    #endregion

    /// <inheritdoc/>
    public int Execute(ServiceFacts criteria) {
      using (var builder = new CommandBuilder(sqlite_connection_)) {
        IEnumerable<int> ids = new ServicesIDsByFacts(sqlite_connection_)
          .Execute(criteria);
        IDbDataParameter parameter;
        var transaction = sqlite_connection_.BeginTransaction();
        IDbCommand cmd = builder
          .SetText(kExecute)
          .AddParameter("@service_id", DbType.Int32, out parameter)
          .SetTransaction(transaction)
          .Build();
        try {
          var removed_services_count = 0;
          foreach (int id in ids) {
            parameter.Value = id;
            cmd.ExecuteScalar();
            removed_services_count++;
          }
          transaction.Commit();
          return removed_services_count;
        } catch (SQLiteException e) {
          try {
            transaction.Rollback();
          } catch (SQLiteException ie) {
            logger_.Error(string.Format(R.Log_MethodThrowsException, "Execute",
              kClassName), e);
            throw new ProviderException(ie);
          }
          logger_.Error(string.Format(R.Log_MethodThrowsException, "Execute",
            kClassName), e);
          throw new ProviderException(e);
        }
      }
    }
  }
}
