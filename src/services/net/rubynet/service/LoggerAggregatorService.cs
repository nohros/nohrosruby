using System;
using Nohros.Data.Json;
using Nohros.Ruby.Logging;

namespace Nohros.Ruby
{
  /// <summary>
  /// An implementation of the <see cref="IAggregatorService"/> that uses
  /// the currently configured logger to log messages.
  /// </summary>
  public class LoggerAggregatorService : IAggregatorService
  {
    readonly IRubyLogger logger_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="LoggerAggregatorService"/>
    /// </summary>
    public LoggerAggregatorService() {
      logger_ = RubyLogger.ForCurrentProcess;
    }
    #endregion

    /// <inheritdoc/>
    public void Log(LogMessage log) {
      logger_.Info(new JsonStringBuilder()
        .WriteBeginObject()
        .WriteMember("application", log.Application)
        .WriteMember("level", log.Level)
        .WriteMember("reason", log.Reason)
        .WriteMember("user", log.User)
        .ToString());
    }
  }
}
