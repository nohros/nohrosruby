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
    readonly IAggregatorLogger logger_;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="LoggerAggregatorDataProvider"/> that uses the current
    /// configured logger to "store" the log messages.
    /// </summary>
    public LoggerAggregatorDataProvider() {
      logger_ = AggregatorLogger.ForCurrentProcess;
    }

    public void Store(LogMessage message) {
      logger_.Info(
        new JsonStringBuilder()
          .WriteBeginObject()
          .WriteMember("level", message.Level)
          .WriteMember("message", message.Message)
          .WriteMember("timestamp", message.TimeStamp)
          .WriteMember("exception", message.Exception)
          .WriteMember("application", message.Application)
          .ToString());
    }
  }
}
