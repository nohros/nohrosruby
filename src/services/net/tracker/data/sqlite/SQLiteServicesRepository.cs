using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Nohros.Ruby.Data.SQLite
{
  public class SQLiteServicesRepository : IServicesRepository
  {
    readonly SQLiteConnection sqlite_connection_;

    #region .ctor
    public SQLiteServicesRepository(SQLiteConnection sqlite_connection) {
      sqlite_connection_ = sqlite_connection;
    }
    #endregion

    public IServicesByFactsQuery Query(out IServicesByFactsQuery query) {
      query = new ServicesByFactsQuery(sqlite_connection_);
      return query;
    }

    public INewServiceCommand Query(out INewServiceCommand command) {
      command = new NewServiceCommand(sqlite_connection_);
      return command;
    }

    public IRemoveServicesCommand Query(out IRemoveServicesCommand query) {
      query = new RemoveServiceQuery(sqlite_connection_);
      return query;
    }

    public void Configure() {
      sqlite_connection_.Open();
      new CreateServiceFactTableCommand(sqlite_connection_).Execute();
      new CreateServiceTableCommand(sqlite_connection_).Execute();
    }
  }
}
