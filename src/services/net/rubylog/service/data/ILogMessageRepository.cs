using System;

namespace Nohros.Ruby.Logging
{
  /// <summary>
  /// A repository for aggregated log messages.
  /// </summary>
  public interface ILogMessageRepository
  {
    /// <summary>
    /// Creates an instance of the <see cref="ILogMessageCommand"/> query
    /// for the associated repository.
    /// </summary>
    /// <param name="query">
    /// A <see cref="ILogMessageCommand"/> object taht can be used to log
    /// messages through the associated repository.
    /// </param>
    ILogMessageCommand Query(out ILogMessageCommand query);

    /// <summary>
    /// Creates an instance of the <see cref="ISetupStorageCommand"/> query
    /// for the associate repository.
    /// </summary>
    /// <param name="query">
    /// A <see cref="ISetupStorageCommand"/> object that can be used to setup
    /// the storage for an application.
    /// </param>
    ISetupStorageCommand Query(out ISetupStorageCommand query);
  }
}
