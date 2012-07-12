using System;
using Nohros.Data.Json;

namespace Nohros.Ruby.Logging
{
  /// <summary>
  /// An implementation of the <see cref="IAggregatorDataProvider"/> class
  /// that does not really stores the log messages. It just forwards the
  /// messages to the configured logger.
  /// </summary>
  public class LoggerAggregatorDataProvider : IAggregatorDataProvider
  {
    readonly IRubyLogger logger_;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="LoggerAggregatorDataProvider"/> that uses the current
    /// configured logger to "store" the log messages.
    /// </summary>
    public LoggerAggregatorDataProvider() {
      logger_ = RubyLogger.ForCurrentProcess;
    }

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
  }
}
