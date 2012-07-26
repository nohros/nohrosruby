using System;
using Nohros.Logging;

namespace Nohros.Ruby
{
  internal class RubyLogger : ForwardingLogger, IRubyLogger
  {
    readonly static RubyLogger current_process_logger_;

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
    public RubyLogger(ILogger logger) : base(logger) {
    }
    #endregion

    /// <summary>
    /// Gets or sets the current application logger.
    /// </summary>
    public static RubyLogger ForCurrentProcess {
      get { return current_process_logger_; }
    }
  }
}
