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

    public IEnumerable<ZMQEndPoint> Query(ServiceFacts criteria) {
      return new ServicesByFactsQuery(sqlite_connection_).Execute(criteria);
    }

    public void Add(ServiceEndpoint criteria) {
      new AddServiceCommand(sqlite_connection_).Execute(criteria);
    }

    public void Remove(ServiceFacts criteria) {
      new RemoveServiceCommand(sqlite_connection_).Execute(criteria);
    }

    public void Configure() {
      sqlite_connection_.Open();
      new CreateServiceFactTableCommand(sqlite_connection_)
        .Execute(new NewServiceFactTableCriteria());
      new CreateServiceTableCommand(sqlite_connection_)
        .Execute(new NewServiceTableCriteria());
    }
  }
}
