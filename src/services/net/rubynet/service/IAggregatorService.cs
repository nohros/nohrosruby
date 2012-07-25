using System;
using Nohros.Ruby.Logging;

namespace Nohros.Ruby
{
  /// <summary>
  /// A class that log messages using the ruby log aggregator service.
  /// </summary>
  public interface IAggregatorService
  {
    /// <summary>
    /// Send a <see cref="LogMessage"/> to the log aggregator service.
    /// </summary>
    /// <param name="log">
    /// The log message to send.
    /// </param>
    void Log(LogMessage log);
  }
}
