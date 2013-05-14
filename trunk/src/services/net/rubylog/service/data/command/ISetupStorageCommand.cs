using System;

namespace Nohros.Ruby.Logging
{
  /// <summary>
  /// Defines the logging storage settings.
  /// </summary>
  /// <remarks>
  /// The logging storage is the place within a <see cref="ILogMessageRepository"/>
  /// where the log message for a specific application are stored.
  /// </remarks>
  public interface ISetupStorageCommand
  {
    /// <summary>
    /// Setups the storage for an specific application.
    /// </summary>
    /// <param name="info">
    /// The information about the storage to configure.
    /// </param>
    /// <returns>
    /// <c>true</c> if the storage was successfully configured; otherwise,
    /// <c>false</c>.
    /// </returns>
    bool Execute(StorageInfo info);
  }
}
