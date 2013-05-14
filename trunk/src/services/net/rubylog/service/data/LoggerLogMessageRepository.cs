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
    class LogMessageCommand : ILogMessageCommand
    {
      readonly LocalLogger logger_;

      #region .ctor
      public LogMessageCommand(LocalLogger logger) {
        logger_ = logger;
      }
      #endregion

      public void Execute(LogMessage message) {
        logger_.Info(
          new JsonStringBuilder()
            .WriteBeginObject()
            .WriteMember("level", message.Level)
            .WriteMember("reason", message.Reason)
            .WriteMember("timestamp", message.TimeStamp)
            .WriteMember("application", message.Application)
            .ToString());
      }
    }

    class SetupStorageCommand : ISetupStorageCommand
    {
      #region .ctor
      public SetupStorageCommand() {
      }
      #endregion

      public bool Execute(StorageInfo info) {
        return true;
      }
    }

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

    /// <inheritdoc/>
    public ILogMessageCommand Query(out ILogMessageCommand query) {
      return query = new LogMessageCommand(logger_);
    }

    /// <inheritdoc/>
    public ISetupStorageCommand Query(out ISetupStorageCommand query) {
      return query = new SetupStorageCommand();
    }
  }
}
