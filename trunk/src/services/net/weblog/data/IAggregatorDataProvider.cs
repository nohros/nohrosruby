using System;

namespace Nohros.Ruby.Logging
{
  /// <summary>
  /// A class used to persist the aggrgated messages to a database.
  /// </summary>
  public interface IAggregatorDataProvider
  {
    /// <summary>
    /// Store a <see cref="LogMessage"/> to the database.
    /// </summary>
    /// <param name="message">
    /// The message to be stored.
    /// </param>
    bool Store(LogMessage message);
  }
}
