using System;
using Nohros.Logging;

namespace Nohros.Ruby
{
  public class QueryLogger : ForwardingLogger
  {
    static readonly QueryLogger current_process_logger_;

    #region .ctor
    static QueryLogger() {
      current_process_logger_ = new QueryLogger(new NOPLogger());
    }
    #endregion

    #region .ctor
    public QueryLogger(ILogger logger) : base(logger) {
    }
    #endregion

    public static QueryLogger ForCurrentProcess {
      get { return current_process_logger_; }
    }
  }
}
