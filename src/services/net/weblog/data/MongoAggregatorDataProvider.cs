using System;
using MongoDB.Driver;
using MongoDB.Bson;

using Nohros.Data.Json;
using Nohros.Resources;

namespace Nohros.Ruby.Logging
{
  public partial class MongoAggregatorDataProvider : IAggregatorDataProvider
  {
    const string kClassName = "Nohros.Ruby.Logging.MongoAggregatorDataProvider";

    readonly IRubyLogger logger_;
    readonly MongoDatabase database_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="MongoAggregatorDataProvider"/> class by using the specified
    /// <see cref="MongoDatabase"/> object.
    /// </summary>
    /// <param name="database">
    /// A <see cref="MongoDatabase"/> object that is used to store the log
    /// messages.
    /// </param>
    public MongoAggregatorDataProvider(MongoDatabase database) {
      database_ = database;
      logger_ = RubyLogger.ForCurrentProcess;
    }
    #endregion

    /// <inheritdoc/>
    public bool Store(LogMessage message) {
      var categorization_document = new BsonDocument();
      var categorization = message.CategorizationList;
      for (int i = 0, j = categorization.Count; i < j; i++) {
        var pair = categorization[i];
        categorization_document.Add(new BsonElement(pair.Key, pair.Value));
      }

      var document = new BsonDocument {
        new BsonElement("application", message.Application),
        new BsonElement("level", message.Level),
        new BsonElement("reason", message.Reason),
        new BsonElement("timestamp", message.TimeStamp),
        new BsonElement("user", message.User),
        new BsonElement("categorization", categorization_document)
      };

      try {
        var collection = database_.GetCollection("logging");
        collection.Insert(document);
      } catch(Exception exception) {
        logger_.Error(
          string.Format(StringResources.Log_MethodThrowsException, kClassName,
            "Store"), exception);
        return false;
      }
      return true;
    }
  }
}
