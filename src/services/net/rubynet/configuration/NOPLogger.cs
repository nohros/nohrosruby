using System;
using System.Collections.Generic;
using Nohros.Logging;

namespace Nohros.Ruby
{
  /// <summary>
  /// An implementation of the <see cref="IRubyLogger"/> that log messages
  /// message.
  /// </summary>
  public class NOPLogger : IRubyLogger
  {
    readonly ILogger logger_;

    #region .ctor
    /// <summary>
    /// Initializes a nes instance if the <see cref="NOPLogger"/>
    /// </summary>
    public NOPLogger() {
      logger_ = new Nohros.Logging.NOPLogger();
    }
    #endregion

    /// <inheritdoc/>
    public void Debug(string message) {
    }

    /// <inheritdoc/>
    public void Debug(string message, Exception exception) {
    }

    /// <inheritdoc/>
    public void Error(string message) {
    }

    /// <inheritdoc/>
    public void Error(string message, Exception exception) {
    }

    /// <inheritdoc/>
    public void Fatal(string message) {
    }

    /// <inheritdoc/>
    public void Fatal(string message, Exception exception) {
    }

    /// <inheritdoc/>
    public void Info(string message) {
    }

    /// <inheritdoc/>
    public void Info(string message, Exception exception) {
    }

    /// <inheritdoc/>
    public void Warn(string message) {
    }

    /// <inheritdoc/>
    public void Warn(string message, Exception exception) {
    }

    /// <inheritdoc/>
    public bool IsDebugEnabled {
      get { return false; }
    }

    /// <inheritdoc/>
    public bool IsErrorEnabled {
      get { return false; }
    }

    /// <inheritdoc/>
    public bool IsFatalEnabled {
      get { return false; }
    }

    /// <inheritdoc/>
    public bool IsInfoEnabled {
      get { return false; }
    }

    /// <inheritdoc/>
    public bool IsWarnEnabled {
      get { return false; }
    }

    /// <inheritdoc/>
    public bool IsTraceEnabled {
      get { return false; }
    }

    /// <inheritdoc/>
    public void Debug(string message, Exception exception,
      IDictionary<string, string> categorization) {
    }

    /// <inheritdoc/>
    public void Debug(string message, IDictionary<string, string> categorization) {
    }

    /// <inheritdoc/>
    public void Error(string message, IDictionary<string, string> categorization) {
    }

    /// <inheritdoc/>
    public void Error(string message, Exception exception,
      IDictionary<string, string> categorization) {
    }

    /// <inheritdoc/>
    public void Fatal(string message, IDictionary<string, string> categorization) {
    }

    /// <inheritdoc/>
    public void Fatal(string message, Exception exception,
      IDictionary<string, string> categorization) {
    }

    /// <inheritdoc/>
    public void Info(string message, IDictionary<string, string> categorization) {
    }

    /// <inheritdoc/>
    public void Info(string message, Exception exception,
      IDictionary<string, string> categorization) {
    }

    /// <inheritdoc/>
    public void Warn(string message, IDictionary<string, string> categorization) {
    }

    /// <inheritdoc/>
    public void Warn(string message, Exception exception,
      IDictionary<string, string> categorization) {
    }

    /// <inheritdoc/>
    public ILogger Logger {
      get { return logger_; }
      set { }
    }
  }
}
