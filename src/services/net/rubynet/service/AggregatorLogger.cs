using System;
using System.Collections.Generic;
using Nohros.Logging;
using Nohros.Ruby.Logging;

namespace Nohros.Ruby
{
  internal class AggregatorLogger : IRubyLogger
  {
    readonly string application_;
    readonly IRubySettings settings_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="AggregatorLogger"/> class
    /// using the specified application name, aggregator_service and log level.
    /// </summary>
    /// <param name="application">
    /// The name of the application associated with the logger.
    /// </param>
    public AggregatorLogger(string application, IRubySettings settings) {
      application_ = application;
      settings_ = settings;
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
    public void Debug(string message, IDictionary<string, string> categorization) {
      Log(message, LogLevel.Debug, categorization);
    }

    /// <inheritdoc/>
    public void Debug(string message, Exception exception,
      IDictionary<string, string> categorization) {
      Log(message, LogLevel.Debug, exception, categorization);
    }

    /// <inheritdoc/>
    public void Error(string message, IDictionary<string, string> categorization) {
      Log(message, LogLevel.Error, categorization);
    }

    /// <inheritdoc/>
    public void Error(string message, Exception exception,
      IDictionary<string, string> categorization) {
      Log(message, LogLevel.Error, exception, categorization);
    }

    /// <inheritdoc/>
    public void Fatal(string message, IDictionary<string, string> categorization) {
      Log(message, LogLevel.Fatal, categorization);
    }

    /// <inheritdoc/>
    public void Fatal(string message, Exception exception,
      IDictionary<string, string> categorization) {
      Log(message, LogLevel.Fatal, exception, categorization);
    }

    /// <inheritdoc/>
    public void Info(string message, IDictionary<string, string> categorization) {
      Log(message, LogLevel.Info, categorization);
    }

    /// <inheritdoc/>
    public void Info(string message, Exception exception,
      IDictionary<string, string> categorization) {
      Log(message, LogLevel.Info, exception, categorization);
    }

    /// <inheritdoc/>
    public void Warn(string message, IDictionary<string, string> categorization) {
      Log(message, LogLevel.Warn, categorization);
    }

    /// <inheritdoc/>
    public void Warn(string message, Exception exception,
      IDictionary<string, string> categorization) {
      Log(message, LogLevel.Warn, exception, categorization);
    }

    /// <inheritdoc/>
    public bool IsDebugEnabled {
      get { return settings_.ServiceLoggerLevel <= LogLevel.Debug; }
    }

    /// <inheritdoc/>
    public bool IsErrorEnabled {
      get { return settings_.ServiceLoggerLevel <= LogLevel.Error; }
    }

    /// <inheritdoc/>
    public bool IsFatalEnabled {
      get { return settings_.ServiceLoggerLevel <= LogLevel.Fatal; }
    }

    /// <inheritdoc/>
    public bool IsInfoEnabled {
      get { return settings_.ServiceLoggerLevel <= LogLevel.Info; }
    }

    /// <inheritdoc/>
    public bool IsWarnEnabled {
      get { return settings_.ServiceLoggerLevel <= LogLevel.Warn; }
    }

    /// <inheritdoc/>
    public bool IsTraceEnabled {
      get { return settings_.ServiceLoggerLevel <= LogLevel.Trace; }
    }

    void Log(string message, LogLevel level) {
      LogMessage.Builder builder =
        GetLogMessageBuilder(message, GetLogLevel(level));
      settings_.AggregatorService.Log(builder.Build());
    }

    void Log(string message, LogLevel level,
      IDictionary<string, string> categorization) {
      LogMessage.Builder builder =
        GetLogMessageBuilder(message, GetLogLevel(level))
          .AddRangeCategorization(KeyValuePairs.FromKeyValuePairs(categorization));
      settings_.AggregatorService.Log(builder.Build());
    }

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
      settings_.AggregatorService.Log(builder.Build());
    }

    void Log(string message, LogLevel level, Exception exception,
      IDictionary<string, string> categorization) {
      LogMessage.Builder builder =
        GetLogMessageBuilder(message, GetLogLevel(level))
          .AddCategorization(new KeyValuePair.Builder()
            .SetKey("exception")
            .SetValue(exception.Message)
            .Build())
          .AddCategorization(new KeyValuePair.Builder()
            .SetKey("backtrace")
            .SetValue(exception.StackTrace)
            .Build())
          .AddRangeCategorization(KeyValuePairs.FromKeyValuePairs(categorization));
      settings_.AggregatorService.Log(builder.Build());
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
