using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Nohros.Ruby.Data.SQLite
{
  public class SQLiteServicesRepositoryFactory : IServicesRepositoryFactory
  {
    /// <inheritdoc/>
    public IServicesRepository CreateServicesRepository(
      IDictionary<string, string> options) {
      var sqlite_connection = new SQLiteConnection("Data Source=:memory:;");
      var repository = new SQLiteServicesRepository(sqlite_connection);
      repository.Initialize();
      return repository;
    }
  }
}
