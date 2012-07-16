using System;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using Nohros.Resources;

namespace Nohros.Ruby.Logging
{
  public partial class MongoAggregatorDataProvider : IAggregatorDataProvider
  {
    const string kClassName = "Nohros.Ruby.Logging.MongoAggregatorDataProvider";

    const string kStorageCollectionName = "storages";
    const string kStorageApplicationField = "app";
    const string kStorageNameField = "name";
    const string kStoragePrefix = "logging.";

    // The estimated size a single log message(in bytes).
    const int kLogMessageSize = 3000;

    readonly MongoDatabase database_;
    readonly IRubyLogger logger_;

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

      var document = new BsonDocument
      {
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
          string.Format(StringResources.Log_MethodThrowsException, kClassName,
            "Store"), exception);
        return false;
      }
      return true;
    }

    /// <inheritdoc/>
    public bool SetupStorage(StorageInfo storage) {
      if (!(storage.HasName
        && storage.Name.Length > 0
          && storage.HasApplication
            && storage.Application.Length > 0)) {
        return false;
      }

      try {
        var collection = database_.GetCollection(storage.Name);
        bool collection_exists = collection.Exists();

        if (collection_exists) {
          // If the storage is not capped, try to convert it.
          if (!collection.IsCapped() && storage.Size > 0) {
            var cmd = new CommandDocument
            {
              {"convertToCapped", storage.Name},
              {"size", kLogMessageSize*storage.Size*2},
              {"max", storage.Size}
            };
            database_.RunCommand("convertToCapped");
          }
        } else {
          // Create the collection only if it should be capped; otherwise, this
          // is a normal collection and it is automatically created by the
          // server at first insert.
          if (storage.Size > 0) {
            var options = new CollectionOptionsDocument
            {
              {"capped", true},
              {"size", kLogMessageSize*storage.Size*2},
              {"max", storage.Size}
            };
            database_.CreateCollection(storage.Name);
          }
        }

        // Associates the application with the storage.
        collection = database_.GetCollection(kStorageCollectionName);
        SafeModeResult result = collection.Save(new BsonDocument {
          {kStorageNameField, storage.Name},
          {kStorageApplicationField, storage.Application}
        });
        return result.Ok;
      } catch (Exception exception) {
        logger_.Error(string.Format(StringResources.Log_MethodThrowsException,
          kClassName, "SetupStorage"), exception);
        return false;
      }
    }

    MongoCollection GetStorage(string application) {
      var name = application;
      var collection = database_.GetCollection(kStorageCollectionName);
      var storage = collection.FindOne(
        Query.EQ(kStorageApplicationField, application));
      if (storage != null) {
        name = storage[kStorageNameField].AsString;
        collection = database_.GetCollection(kStoragePrefix + name);
        if (collection.Exists()) {
          return collection;
        }
      }
      return CreateStorage(kStoragePrefix + name);
    }

    MongoCollection CreateStorage(string name) {
      CommandResult result = database_.CreateCollection(name);
      if (result.Ok) {
        // TODO (neylor.silva): Add the default index to the collection.
      }
      return database_.GetCollection(name);
    }
  }
}
