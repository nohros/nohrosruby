using System;

using Nohros.Logging;

namespace Nohros.Ruby
{
  /// <summary>
  /// A simple logging interface abstracting logging APIs.
  /// </summary>
  /// <remarks>
  /// <para>
  /// This interface is very simple and does not expose some advanced fetures that some logging
  /// libraries provides. For example, log4net provides methods that format the logging message
  /// based on some arguments, this interface does not declare such methods, because this could
  /// be done by the client code and could conflict with libraries that do not implements this
  /// type of functionality.
  /// </para>
  /// <para>
  /// The internal logging library that was used by the this project could change and projects
  /// that depends on this library will stop working. This interface provides an abstraction
  /// around the logging API and garantee that changes on the underlying loggin library does
  /// not affect app codes the must framework.
  /// </para>
  /// <para>
  /// The documentation of this class and the methods was based on the Apache log4net library. At the
  /// time of write log4net is the underlying library used by the must framework.
  /// </para>
  /// </remarks>
  internal class RubyLogger: IRubyLogger
  {
    static IRubyLogger current_process_logger_;

    ILogger internal_logger_;

    #region .ctor

    /// <summary>
    /// Initializes the singleton process's logger instance.
    /// </summary>
    static RubyLogger() {
      // set the logger to be used, configure it and
      // do some initialization stuffs
      //
      // ex.
      //   FileLogger internal_logger =
      //     FileLogger.ForCurrentProcess();

      // creates a instance of the NOPLogger to ensure that
      // the ForCurrentProcess always returns a valid logger.
      // Note that the client is responsible to set the value
      // of the singleton logger object.
      NOPLogger internal_logger = new NOPLogger();

      // initialize a new instance of the RubyLogger using the
      // previously instantiated logger.
      RubyLogger logger = new RubyLogger(internal_logger);

      current_process_logger_ = logger as IRubyLogger;
    }
    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="RubyLogger"/>
    /// class by using the specified <see cref="ILogger"/> interface.
    /// </summary>
    public RubyLogger(ILogger logger) {
      internal_logger_ = logger;
    }

    /// <summary>
    /// Gets or sets the current application logger.
    /// </summary>
    public static IRubyLogger ForCurrentProcess {
      get {
        return current_process_logger_;
      }
      internal set {
        current_process_logger_ = value;
      }
    }

    #region IsEnabled

    /// <inherit />
    public bool IsDebugEnabled {
      get {
        return internal_logger_.IsDebugEnabled;
      }
    }

    /// <inherit />
    public bool IsErrorEnabled {
      get {
        return internal_logger_.IsErrorEnabled;
      }
    }

    /// <inherit />
    public bool IsFatalEnabled {
      get {
        return internal_logger_.IsFatalEnabled;
      }
    }

    /// <inherit />
    public bool IsInfoEnabled {
      get {
        return internal_logger_.IsInfoEnabled;
      }
    }

    /// <inherit />
    public bool IsWarnEnabled {
      get {
        return internal_logger_.IsWarnEnabled;
      }
    }

    /// <inherit />
    public bool IsTraceEnabled {
      get {
        return internal_logger_.IsTraceEnabled;
      }
    }

    #endregion

    /// <inherit />
    public void Debug(string message) {
      internal_logger_.Debug(message);
    }

    /// <inherit />
    public void Debug(string message, Exception exception) {
      internal_logger_.Debug(message, exception);
    }

    /// <inherit />
    public void Error(string message) {
      internal_logger_.Error(message);
    }

    /// <inherit />
    public void Error(string message, Exception exception) {
      internal_logger_.Error(message, exception);
    }

    /// <inherit />
    public void Fatal(string message) {
      internal_logger_.Fatal(message);
    }

    /// <inherit />
    public void Fatal(string message, Exception exception) {
      internal_logger_.Fatal(message, exception);
    }

    /// <inherit />
    public void Info(string message) {
      internal_logger_.Info(message);
    }

    /// <inherit />
    public void Info(string message, Exception exception) {
      internal_logger_.Info(message, exception);
    }

    /// <inherit />
    public void Warn(string message) {
      internal_logger_.Warn(message);
    }

    /// <inherit />
    public void Warn(string message, Exception exception) {
      internal_logger_.Warn(message, exception);
    }
  }
}