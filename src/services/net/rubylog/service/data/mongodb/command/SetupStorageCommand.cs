using System;
using MongoDB.Bson;
using MongoDB.Driver;
using R = Nohros.Resources.StringResources;

namespace Nohros.Ruby.Logging.Data.MongoDB
{
  public class SetupStorageCommand : ISetupStorageCommand
  {
    const string kClassName =
      "Nohros.Ruby.Logging.Data.MongoDB.SetupStorageCommand";

    const int kLogMessageSize = 3000;
    const string kStorageCollectionName = "storages";
    const string kStorageApplicationField = "app";
    const string kStorageNameField = "name";

    readonly MongoDatabase database_;
    readonly LocalLogger logger_;

    #region .ctor
    public SetupStorageCommand(MongoDatabase database) {
      database_ = database;
      logger_ = LocalLogger.ForCurrentProcess;
    }
    #endregion

    public bool Execute(StorageInfo storage) {
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
            var cmd = new CommandDocument {
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
            var options = new CollectionOptionsDocument {
              {"capped", true},
              {"size", kLogMessageSize*storage.Size*2},
              {"max", storage.Size}
            };
            database_.CreateCollection(storage.Name);
          }
        }

        // Associates the application with the storage.
        collection = database_.GetCollection(kStorageCollectionName);
        WriteConcernResult result = collection.Save(
          new BsonDocument {
            {kStorageNameField, storage.Name},
            {kStorageApplicationField, storage.Application}
          });
        return result.Ok;
      } catch (Exception exception) {
        logger_.Error(string.Format(R.Log_MethodThrowsException,
          kClassName, "SetupStorage"), exception);
        return false;
      }
    }
  }
}
