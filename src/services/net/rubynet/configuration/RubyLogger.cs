using System;
using Nohros.Logging;

namespace Nohros.Ruby
{
  internal class RubyLogger: ForwardingLogger, IRubyLogger
  {
    readonly static RubyLogger current_process_logger_;
    IRubyLogger logger_;

    #region .ctor
    /// <summary>
    /// Initializes the singleton process's logger instance.
    /// </summary>
    static RubyLogger() {
      current_process_logger_ = new RubyLogger(new NOPLogger());
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RubyLogger"/>
    /// class by using the specified <see cref="ILogger"/> interface.
    /// </summary>
    public RubyLogger(IRubyLogger logger)
      : base(logger) {
      logger_ = logger;
    }
    #endregion

    /// <summary>
    /// Gets or sets the current application logger.
    /// </summary>
    public static RubyLogger ForCurrentProcess {
      get { return current_process_logger_; }
    }

    /// <inheritdoc/>
    public void Debug(string message, string service) {
      logger_.Debug(message, service);
    }

    /// <inheritdoc/>
    public void Debug(string message, Exception exception, string service) {
      logger_.Debug(message, exception, service);
    }

    /// <inheritdoc/>
    public void Error(string message, string service) {
      logger_.Error(message, service);
    }

    /// <inheritdoc/>
    public void Error(string message, Exception exception, string service) {
      logger_.Error(message, exception, service);
    }

    /// <inheritdoc/>
    public void Fatal(string message, string service) {
      logger_.Fatal(message, service);
    }

    /// <inheritdoc/>
    public void Fatal(string message, Exception exception, string service) {
      logger_.Fatal(message, exception, service);
    }

    /// <inheritdoc/>
    public void Info(string message, string service) {
      logger_.Info(message, service);
    }

    /// <inheritdoc/>
    public void Info(string message, Exception exception, string service) {
      logger_.Info(message, exception, service);
    }

    /// <inheritdoc/>
    public void Warn(string message, string service) {
      logger_.Warn(message, service);
    }

    /// <inheritdoc/>
    public void Warn(string message, Exception exception, string service) {
      logger_.Warn(message, exception, service);
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
