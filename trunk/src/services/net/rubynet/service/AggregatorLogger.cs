using System;

using Nohros.Logging;
using Nohros.Ruby.Logging;

namespace Nohros.Ruby
{
  internal class AggregatorLogger : IRubyLogger
  {
    readonly string application_;
    IAggregatorService aggregator_service_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="AggregatorLogger"/> class
    /// using the specified application name, aggregator_service and log level.
    /// </summary>
    /// <param name="application">
    /// The name of the application associated with the logger.
    /// </param>
    /// <param name="aggregator_service">
    /// A <see cref="AggregatorService"/> object that is used to send the log
    /// message to the log aggregator_service service.
    /// </param>
    public AggregatorLogger(string application,
      IAggregatorService aggregator_service) {
      aggregator_service_ = aggregator_service;
      application_ = application;
    }
    #endregion

    /// <inheritdoc/>
    public void Debug(string message) {
      Log(message, LogLevel.Debug);
    }

    /// <inheritdoc/>
    public void Debug(string message, Exception exception) {
      Log(message, LogLevel.Debug, exception);
    }

    /// <inheritdoc/>
    public void Error(string message) {
      Log(message, LogLevel.Error);
    }

    /// <inheritdoc/>
    public void Error(string message, Exception exception) {
      Log(message, LogLevel.Error, exception);
    }

    /// <inheritdoc/>
    public void Fatal(string message) {
      Log(message, LogLevel.Fatal);
    }

    /// <inheritdoc/>
    public void Fatal(string message, Exception exception) {
      Log(message, LogLevel.Fatal, exception);
    }

    /// <inheritdoc/>
    public void Info(string message) {
      Log(message, LogLevel.Info);
    }

    /// <inheritdoc/>
    public void Info(string message, Exception exception) {
      Log(message, LogLevel.Info, exception);
    }

    /// <inheritdoc/>
    public void Warn(string message) {
      Log(message, LogLevel.Warn);
    }

    /// <inheritdoc/>
    public void Warn(string message, Exception exception) {
      Log(message, LogLevel.Warn, exception);
    }

    /// <inheritdoc/>
    public bool IsDebugEnabled {
      get { return RubyLogger.ForCurrentProcess.IsDebugEnabled; }
    }

    /// <inheritdoc/>
    public bool IsErrorEnabled {
      get { return RubyLogger.ForCurrentProcess.IsErrorEnabled; }
    }

    /// <inheritdoc/>
    public bool IsFatalEnabled {
      get { return RubyLogger.ForCurrentProcess.IsFatalEnabled; }
    }

    /// <inheritdoc/>
    public bool IsInfoEnabled {
      get { return RubyLogger.ForCurrentProcess.IsInfoEnabled; }
    }

    /// <inheritdoc/>
    public bool IsWarnEnabled {
      get { return RubyLogger.ForCurrentProcess.IsWarnEnabled; }
    }

    /// <inheritdoc/>
    public bool IsTraceEnabled {
      get { return RubyLogger.ForCurrentProcess.IsTraceEnabled; }
    }

    /// <summary>
    /// Gets or sets the <see cref="IAggregatorService"/> object that is used
    /// to communicate with the aggregator service.
    /// </summary>
    public IAggregatorService AggregatorService {
      get { return aggregator_service_; }
      set { aggregator_service_ = value; }
    }

    /// <inheritdoc/>
    void Log(string message, LogLevel level) {
      LogMessage.Builder builder = GetLogMessageBuilder(message,
        GetLogLevel(level));
      aggregator_service_.Log(builder.Build());
    }

    /// <inheritdoc/>
    void Log(string message, LogLevel level, Exception exception) {
      LogMessage.Builder builder =
        GetLogMessageBuilder(message, GetLogLevel(level))
          .AddCategorization(new KeyValuePair.Builder()
            .SetKey("exception")
            .SetValue(exception.Message)
            .Build())
          .AddCategorization(new KeyValuePair.Builder()
            .SetKey("backtrace")
            .SetValue(exception.StackTrace)
            .Build());
      aggregator_service_.Log(builder.Build());
    }

    string GetLogLevel(LogLevel level) {
      switch (level) {
        case LogLevel.Debug:
          return "debug";
        case LogLevel.Info:
          return "info";
        case LogLevel.All:
          return "all";
        case LogLevel.Trace:
          return "trace";
        case LogLevel.Fatal:
          return "fatal";
        case LogLevel.Off:
          return "off";
        case LogLevel.Warn:
          return "warn";
        case LogLevel.Error:
          return "error";
      }
      throw new ArgumentOutOfRangeException("level");
    }

    LogMessage.Builder GetLogMessageBuilder(string message, string level) {
      return new LogMessage.Builder()
        .SetApplication(application_)
        .SetLevel(level)
        .SetReason(message)
        .SetTimeStamp(TimeUnitHelper.ToUnixTime(DateTime.Now))
        .SetUser(Environment.UserName)
        .AddCategorization(new KeyValuePair.Builder()
          .SetKey("host")
          .SetValue(Environment.MachineName)
          .Build())
        .AddCategorization(new KeyValuePair.Builder()
          .SetKey("os")
          .SetValue(Environment.OSVersion.ToString())
          .Build())
        .AddCategorization(new KeyValuePair.Builder()
          .SetKey("clr-version")
          .SetValue(Environment.Version.ToString())
          .Build());
    }
  }
}
