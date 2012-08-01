using System;
using System.Collections.Generic;
using Nohros.Logging;

namespace Nohros.Ruby
{
  internal class RubyLogger : ForwardingLogger, IRubyLogger
  {
    static readonly RubyLogger current_process_logger_;
    IRubyLogger logger_;

    #region .ctor
    /// <summary>
    /// Initializes the singleton process's logger instance.
    /// </summary>
    static RubyLogger() {
      current_process_logger_ = new RubyLogger(new NOPLogger());
    }
    #endregion

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="RubyLogger"/>
    /// class by using the specified <see cref="ILogger"/> interface.
    /// </summary>
    public RubyLogger(IRubyLogger logger)
      : base(logger) {
      logger_ = logger;
    }
    #endregion

    /// <inheritdoc/>
    public void Debug(string message, IDictionary<string, string> categorization) {
      logger_.Debug(message, categorization);
    }

    /// <inheritdoc/>
    public void Debug(string message, Exception exception,
      IDictionary<string, string> categorization) {
      logger_.Debug(message, exception, categorization);
    }

    /// <inheritdoc/>
    public void Error(string message, IDictionary<string, string> categorization) {
      logger_.Error(message, categorization);
    }

    /// <inheritdoc/>
    public void Error(string message, Exception exception,
      IDictionary<string, string> categorization) {
      logger_.Error(message, exception, categorization);
    }

    /// <inheritdoc/>
    public void Fatal(string message, IDictionary<string, string> categorization) {
      logger_.Fatal(message, categorization);
    }

    /// <inheritdoc/>
    public void Fatal(string message, Exception exception,
      IDictionary<string, string> categorization) {
      logger_.Fatal(message, exception, categorization);
    }

    /// <inheritdoc/>
    public void Info(string message, IDictionary<string, string> categorization) {
      logger_.Info(message, categorization);
    }

    /// <inheritdoc/>
    public void Info(string message, Exception exception,
      IDictionary<string, string> categorization) {
      logger_.Info(message, exception, categorization);
    }

    /// <inheritdoc/>
    public void Warn(string message, IDictionary<string, string> categorization) {
      logger_.Warn(message, categorization);
    }

    /// <inheritdoc/>
    public void Warn(string message, Exception exception,
      IDictionary<string, string> categorization) {
      logger_.Warn(message, exception, categorization);
    }

    /// <summary>
    /// Gets or sets the current application logger.
    /// </summary>
    public static RubyLogger ForCurrentProcess {
      get { return current_process_logger_; }
    }

    /// <summary>
    /// Gets the logger that is used to forward <see cref="RubyLogger"/>
    /// methods.
    /// </summary>
    public IRubyLogger BackingLogger {
      get { return logger_; }
      set { logger = logger_ = value; }
    }
  }
}
