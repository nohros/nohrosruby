using System;

namespace Nohros.Ruby.Logging
{
  /// <summary>
  /// A repository for aggregated log messages.
  /// </summary>
  public interface ILogMessageRepository
  {
    /// <summary>
    /// Store a <see cref="LogMessage"/> to the database.
    /// </summary>
    /// <param name="message">
    /// The message to be stored.
    /// </param>
    bool Store(LogMessage message);

    /// <summary>
    /// Sets the storage properties for a particular application.
    /// </summary>
    /// <param name="storage">
    /// A <see cref="StorageInfo"/> object containing the storage properties
    /// to set.
    /// </param>
    /// <returns><c>true</c> if the storage properties was successfully set;
    /// otherwise, <c>false</c>.</returns>
    bool SetupStorage(StorageInfo storage);
  }
}
