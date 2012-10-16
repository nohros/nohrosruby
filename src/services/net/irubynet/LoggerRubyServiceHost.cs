using System;
using System.Collections.Generic;
using Nohros.Ruby.Protocol;

namespace Nohros.Ruby
{
  /// <summary>
  /// A implementtion of the <see cref="IRubyServiceHost"/> that send messages
  /// to a <see cref="IRubyLogger"/>.
  /// </summary>
  public class LoggerRubyServiceHost : IRubyServiceHost
  {
    readonly IRubyLogger logger_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="NopRubyServiceHost"/>.
    /// </summary>
    public LoggerRubyServiceHost(IRubyLogger logger) {
      logger_ = logger;
    }
    #endregion

    public bool Send(IRubyMessage message) {
      throw new NotImplementedException();
    }

    public bool Send(int id, int type, byte[] message) {
      throw new NotImplementedException();
    }

    public IRubyLogger Logger {
      get { return logger_; }
    }

    public bool SendError(int message_id, string error, int exception_code) {
      logger_.Error(exception_code + " " + error, new Dictionary<string, string>
      {
        {"messageId", message_id.ToString()},
      });
      return true;
    }

    public bool SendError(int message_id, string error, int exception_code,
      Exception exception) {
      logger_.Error(exception_code + " " + error, exception,
        new Dictionary<string, string>
        {
          {"messageId", message_id.ToString()},
        });
      return true;
    }

    public bool SendError(int message_id, int exception_code,
      Exception exception) {
      logger_.Error(exception_code.ToString(), exception,
        new Dictionary<string, string>
        {
          {"messageId", message_id.ToString()},
        });
      return true;
    }
  }
}
