using System;
using Nohros.Logging;

namespace Nohros.Ruby.Logging
{
  public class LocalLogger : ForwardingLogger
  {
    static readonly LocalLogger current_process_logger_;

    static LocalLogger() {
      current_process_logger_ = new LocalLogger(new NOPLogger());
    }

    public LocalLogger(ILogger logger) : base(logger) {
    }

    public static LocalLogger ForCurrentProcess {
      get { return current_process_logger_; }
    }
  }
}