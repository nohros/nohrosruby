using System;
using MongoDB.Driver;
using Nohros.Ruby.Logging.Data.MongoDB;
using R = Nohros.Resources.StringResources;

namespace Nohros.Ruby.Logging
{
  public class MongoDataProvider : ILogMessageRepository
  {
    readonly MongoDatabase database_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="MongoDataProvider"/> class by using the specified
    /// <see cref="MongoDatabase"/> object.
    /// </summary>
    /// <param name="database">
    /// A <see cref="MongoDatabase"/> object that is used to store the log
    /// messages.
    /// </param>
    public MongoDataProvider(MongoDatabase database) {
      database_ = database;
    }
    #endregion

    /// <inheritdoc/>
    public ILogMessageCommand Query(out ILogMessageCommand query) {
      return query = new LogMessageCommand(database_);
    }

    /// <inheritdoc/>
    public ISetupStorageCommand Query(out ISetupStorageCommand query) {
      return query = new SetupStorageCommand(database_);
    }
  }
}
