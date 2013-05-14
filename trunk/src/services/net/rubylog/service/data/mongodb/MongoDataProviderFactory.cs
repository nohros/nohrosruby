using System;
using System.Collections.Generic;
using MongoDB.Driver;
using Nohros.Data.MongoDB;

namespace Nohros.Ruby.Logging.Data.MongoDB
{
  public class MongoDataProviderFactory : ILogMessageRepositoryFactory
  {
    public ILogMessageRepository CreateAggregatorDataProvider(
      IDictionary<string, string> options) {
      var factory = new MongoDatabaseProviderFactory();
      MongoDatabaseProvider provider = factory.CreateProvider(options);
      MongoDatabase database = provider.GetDatabase();
      return new MongoDataProvider(database);
    }
  }
}
