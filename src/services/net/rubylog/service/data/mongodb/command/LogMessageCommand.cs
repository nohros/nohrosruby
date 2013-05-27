using System;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using R = Nohros.Resources.StringResources;

namespace Nohros.Ruby.Logging.Data.MongoDB
{
  public class LogMessageCommand : ILogMessageCommand
  {
    const string kClassName =
      "Nohros.Ruby.Logging.Data.MongoDB.LogMessageCommand";

    const string kStorageCollectionName = kStoragePrefix + "storages";
    const string kStorageApplicationField = "app";
    const string kStorageNameField = "name";
    const string kStoragePrefix = "logging.";

    readonly MongoDatabase database_;
    readonly LocalLogger logger_;

    #region .ctor
    public LogMessageCommand(MongoDatabase database) {
      database_ = database;
      logger_ = LocalLogger.ForCurrentProcess;
    }
    #endregion

    public void Execute(LogMessage message) {
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
        var storage = GetStorage(message.Application);
        storage.Insert(document);
      } catch (Exception exception) {
        logger_.Error(
          string.Format(R.Log_MethodThrowsException, kClassName,
            "Store"), exception);
      }
    }

    MongoCollection GetStorage(string application) {
      var name = SanitizeCollectionName(application);
      var collection = database_.GetCollection(kStorageCollectionName);
      var storage = collection.FindOne(
        Query.EQ(kStorageApplicationField, name));
      if (storage != null) {
        name = storage[kStorageNameField].AsString;
        collection = database_.GetCollection(kStoragePrefix + name);
        if (collection.Exists()) {
          return collection;
        }
      }
      return CreateStorage(kStoragePrefix + name);
    }

    string SanitizeCollectionName(string name) {
      return name.Replace(" ", ".").ToLower();
    }

    MongoCollection CreateStorage(string name) {
      var storage = database_.GetCollection(name);
      if (!storage.Exists()) {
        storage.EnsureIndex(
          new IndexKeysBuilder()
            .Ascending("timestamp"));
        storage.EnsureIndex(
          new IndexKeysBuilder()
            .Ascending("level"));
      }
      return storage;
    }
  }
}
