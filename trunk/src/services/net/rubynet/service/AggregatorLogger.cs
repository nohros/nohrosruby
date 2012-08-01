using System;
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
      Log(message, LogLevel.Debug, application_);
    }

    /// <inheritdoc/>
    public void Debug(string message, Exception exception) {
      Log(message, LogLevel.Debug, exception, application_);
    }

    /// <inheritdoc/>
    public void Error(string message) {
      Log(message, LogLevel.Error, application_);
    }

    /// <inheritdoc/>
    public void Error(string message, Exception exception) {
      Log(message, LogLevel.Error, exception, application_);
    }

    /// <inheritdoc/>
    public void Fatal(string message) {
      Log(message, LogLevel.Fatal, application_);
    }

    /// <inheritdoc/>
    public void Fatal(string message, Exception exception) {
      Log(message, LogLevel.Fatal, exception, application_);
    }

    /// <inheritdoc/>
    public void Info(string message) {
      Log(message, LogLevel.Info, application_);
    }

    /// <inheritdoc/>
    public void Info(string message, Exception exception) {
      Log(message, LogLevel.Info, exception, application_);
    }

    /// <inheritdoc/>
    public void Warn(string message) {
      Log(message, LogLevel.Warn, application_);
    }

    /// <inheritdoc/>
    public void Warn(string message, Exception exception) {
      Log(message, LogLevel.Warn, exception, application_);
    }

    /// <inheritdoc/>
    public void Debug(string message, string service) {
      Log(message, LogLevel.Debug, GetApplication(service));
    }

    /// <inheritdoc/>
    public void Debug(string message, Exception exception, string service) {
      Log(message, LogLevel.Debug, exception, GetApplication(service));
    }

    /// <inheritdoc/>
    public void Error(string message, string service) {
      Log(message, LogLevel.Error, GetApplication(service));
    }

    /// <inheritdoc/>
    public void Error(string message, Exception exception, string service) {
      Log(message, LogLevel.Error, exception, GetApplication(service));
    }

    /// <inheritdoc/>
    public void Fatal(string message, string service) {
      Log(message, LogLevel.Fatal, GetApplication(service));
    }

    /// <inheritdoc/>
    public void Fatal(string message, Exception exception, string service) {
      Log(message, LogLevel.Fatal, exception, GetApplication(service));
    }

    /// <inheritdoc/>
    public void Info(string message, string service) {
      Log(message, LogLevel.Info, GetApplication(service));
    }

    /// <inheritdoc/>
    public void Info(string message, Exception exception, string service) {
      Log(message, LogLevel.Info, exception, GetApplication(service));
    }

    /// <inheritdoc/>
    public void Warn(string message, string service) {
      Log(message, LogLevel.Warn, GetApplication(service));
    }

    /// <inheritdoc/>
    public void Warn(string message, Exception exception, string service) {
      Log(message, LogLevel.Warn, exception, GetApplication(service));
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

    string GetApplication(string service) {
      return application_ + "." + service;
    }

    /// <inheritdoc/>
    void Log(string message, LogLevel level, string application) {
      LogMessage.Builder builder = GetLogMessageBuilder(message,
        GetLogLevel(level), application);
      settings_.AggregatorService.Log(builder.Build());
    }

    /// <inheritdoc/>
    void Log(string message, LogLevel level, Exception exception, string application) {
      LogMessage.Builder builder =
        GetLogMessageBuilder(message, GetLogLevel(level), application)
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

    LogMessage.Builder GetLogMessageBuilder(string message, string level, string application) {
      return new LogMessage.Builder()
        .SetApplication(application)
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
