using System;
using Nohros.Ruby.Protocol;

namespace Nohros.Ruby
{
  /// <summary>
  /// A implementtion of the <see cref="IRubyServiceHost"/> that do nothing.
  /// </summary>
  public class NopRubyServiceHost : IRubyServiceHost
  {
    readonly IRubyLogger logger_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="NopRubyServiceHost"/>.
    /// </summary>
    public NopRubyServiceHost() {
      logger_ = new NopRubyLogger();
    }
    #endregion

    public bool Send(IRubyMessage message) {
      return false;
    }

    public bool Send(byte[] message_id, int type, byte[] message,
      byte[] destination) {
      return false;
    }

    public IRubyLogger Logger {
      get { return logger_; }
    }

    public bool SendError(byte[] message_id, int exception_code, string error,
      byte[] destination) {
      return false;
    }

    public bool SendError(byte[] message_id, int exception_code, string error,
      byte[] destination, Exception exception) {
      return false;
    }

    public bool SendError(byte[] message_id, int exception_code,
      byte[] destination, Exception exception) {
      return false;
    }

    public bool Send(byte[] message_id, int type, byte[] message,
      byte[] destination, string token) {
      return false;
    }
  }
}
