using System;

namespace Nohros.Ruby.Logging
{
  /// <summary>
  /// Logs a message on the repository.
  /// </summary>
  public interface ILogMessageCommand
  {
    /// <summary>
    /// Logs the message on the repository.
    /// </summary>
    /// <param name="message">
    /// The message to be logged.
    /// </param>
    void Execute(LogMessage message);
  }
}
