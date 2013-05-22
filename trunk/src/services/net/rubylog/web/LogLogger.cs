using System;
using Nohros.Logging;

namespace Nohros.Ruby.Logging
{
  public class LogLogger : ForwardingLogger
  {
    static readonly LogLogger current_process_logger_;

    #region .ctor
    static LogLogger() {
      current_process_logger_ = new LogLogger(new NOPLogger());
    }
    #endregion

    #region .ctor
    public LogLogger(ILogger logger) : base(logger) {
    }
    #endregion

    public static LogLogger ForCurrentProcess {
      get { return current_process_logger_; }
    }
  }
}
