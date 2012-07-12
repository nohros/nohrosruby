using System;
using System.Collections.Generic;
using MongoDB.Driver;

namespace Nohros.Ruby.Logging
{
  public partial class MongoAggregatorDataProvider :
    IAggregatorDataProviderFactory
  {
    const string kConnectionStringOption = "connection-string";
    const string kDatabaseName = "database-name";

    readonly IAggregatorSettings settings_;

    #region .ctor
    /// <summary>
    /// Constructor implied by the <see cref="IAggregatorDataProviderFactory"/>
    /// interface.
    /// </summary>
    protected MongoAggregatorDataProvider(IAggregatorSettings settings) {
      settings_ = settings;
    }
    #endregion

    IAggregatorDataProvider
      IAggregatorDataProviderFactory.CreateAggregatorDataProvider(
      IDictionary<string, string> options) {
      MongoServer server = MongoServer.Create(options[kConnectionStringOption]);
      return
        new MongoAggregatorDataProvider(
          server.GetDatabase(options[kDatabaseName]));
    }
  }
}
