using System;

namespace Nohros.Ruby
{
  /// <summary>
  /// An implementation of the <see cref="IRubyLogger"/> which forwards all
  /// its methods to another <see cref="IRubyLogger"/> object.
  /// </summary>
  public class ForwardingLogger : IRubyLogger
  {
    static readonly ForwardingLogger current_process_logger_;
    IRubyLogger logger_;

    #region .ctor
    /// <summary>
    /// Initializes the singleton process's logger instance with uses the
    /// <see cref=" NoOpLogger"/> as backing logger.
    /// </summary>
    static ForwardingLogger() {
      current_process_logger_ = new ForwardingLogger(new NoOpLogger());
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ForwardingLogger"/> using
    /// the specified <see cref="IRubyLogger"/> as backing logger.
    /// </summary>
    /// <param name="logger">
    /// The logger instance that methods are forwarder to.
    /// </param>
    public ForwardingLogger(IRubyLogger logger) {
      logger_ = logger;
    }
    #endregion

    /// <inheritdoc />
    public bool IsDebugEnabled {
      get { return logger_.IsDebugEnabled; }
    }

    /// <inheritdoc />
    public bool IsErrorEnabled {
      get { return logger_.IsErrorEnabled; }
    }

    /// <inheritdoc />
    public bool IsFatalEnabled {
      get { return logger_.IsFatalEnabled; }
    }

    /// <inheritdoc />
    public bool IsInfoEnabled {
      get { return logger_.IsInfoEnabled; }
    }

    /// <inheritdoc />
    public bool IsWarnEnabled {
      get { return logger_.IsWarnEnabled; }
    }

    /// <inheritdoc />
    public bool IsTraceEnabled {
      get { return logger_.IsTraceEnabled; }
    }

    /// <inheritdoc />
    public void Debug(string message) {
      logger_.Debug(message);
    }

    /// <inheritdoc />
    public void Debug(string message, Exception exception) {
      logger_.Debug(message, exception);
    }

    /// <inheritdoc />
    public void Error(string message) {
      logger_.Error(message);
    }

    /// <inheritdoc />
    public void Error(string message, Exception exception) {
      logger_.Error(message, exception);
    }

    /// <inheritdoc />
    public void Fatal(string message) {
      logger_.Fatal(message);
    }

    /// <inheritdoc />
    public void Fatal(string message, Exception exception) {
      logger_.Fatal(message, exception);
    }

    /// <inheritdoc />
    public void Info(string message) {
      logger_.Info(message);
    }

    /// <inheritdoc />
    public void Info(string message, Exception exception) {
      logger_.Info(message, exception);
    }

    /// <inheritdoc />
    public void Warn(string message) {
      logger_.Warn(message);
    }

    /// <inheritdoc />
    public void Warn(string message, Exception exception) {
      logger_.Warn(message, exception);
    }

    /// <summary>
    /// Gets the backing logger instance that methods are forwarder to.
    /// </summary>
    public IRubyLogger Logger {
      get { return logger_; }
      set { logger_ = value; }
    }

    /// <summary>
    /// Gets or the currently configured application logger.
    /// </summary>
    public static ForwardingLogger ForCurrentProcess {
      get { return current_process_logger_; }
    }
  }
}
