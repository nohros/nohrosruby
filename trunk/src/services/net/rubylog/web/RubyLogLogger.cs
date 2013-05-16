using System;
using Nohros.Logging;

namespace Nohros.Ruby.Logging
{
  public class RubyLogLogger : ForwardingLogger
  {
    static readonly RubyLogLogger current_process_logger_;

    #region .ctor
    static RubyLogLogger() {
      current_process_logger_ = new RubyLogLogger(new NOPLogger());
    }
    #endregion

    #region .ctor
    public RubyLogLogger(ILogger logger) : base(logger) {
    }
    #endregion

    public static RubyLogLogger ForCurrentProcess {
      get { return current_process_logger_; }
    }
  }
}
