using System;
using System.Collections.Generic;
using MongoDB.Driver;

namespace Nohros.Ruby.Logging
{
  public class MongoDataProviderFactory : ILogMessageRepositoryFactory
  {
    const string kConnectionStringOption = "connection-string";
    const string kDatabaseName = "database-name";

    readonly IAggregatorSettings settings_;

    #region .ctor
    /// <summary>
    /// Constructor implied by the <see cref="ILogMessageRepositoryFactory"/>
    /// interface.
    /// </summary>
    protected MongoDataProviderFactory(IAggregatorSettings settings) {
      settings_ = settings;
    }
    #endregion

    /// <inheritdoc/>
    public ILogMessageRepository CreateAggregatorDataProvider(
      IDictionary<string, string> options) {
      var client = new MongoClient(options[kConnectionStringOption]);
      var server = client.GetServer();
      var database = server.GetDatabase((options[kDatabaseName]));
      return new MongoDataProvider(database);
    }
  }
}
