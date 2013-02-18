using System;
using Nohros.Logging;

namespace Nohros.Ruby
{
  public class TrackerLogger : ForwardingLogger
  {
    static readonly TrackerLogger current_process_logger_;

    #region .ctor
    static TrackerLogger() {
      current_process_logger_ = new TrackerLogger(new NOPLogger());
    }
    #endregion

    #region .ctor
    public TrackerLogger(ILogger logger) : base(logger) {
    }
    #endregion

    public static TrackerLogger ForCurrentProcess {
      get { return current_process_logger_; }
    }
  }
}
