using System;
using Nohros.Data.Json;

namespace Nohros.Ruby.Logging
{
  /// <summary>
  /// An implementation of the <see cref="ILogMessageRepository"/> class
  /// that does not really stores the log messages. It just forwards the
  /// messages to the configured logger.
  /// </summary>
  public class LoggerLogMessageRepository : ILogMessageRepository
  {
    readonly LocalLogger logger_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="LoggerLogMessageRepository"/> that uses the current
    /// configured logger to "store" the log messages.
    /// </summary>
    public LoggerLogMessageRepository() {
      logger_ = LocalLogger.ForCurrentProcess;
    }
    #endregion

    public bool Store(LogMessage message) {
      logger_.Info(
        new JsonStringBuilder()
          .WriteBeginObject()
          .WriteMember("level", message.Level)
          .WriteMember("reason", message.Reason)
          .WriteMember("timestamp", message.TimeStamp)
          .WriteMember("application", message.Application)
          .ToString());
      return true;
    }

    public bool SetupStorage(StorageInfo storage) {
      return true;
    }
  }
}
