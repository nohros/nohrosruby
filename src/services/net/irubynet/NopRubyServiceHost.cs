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

    public bool Send(byte[] data, int type, int id) {
      return false;
    }

    public IRubyLogger Logger {
      get { return logger_; }
    }
  }
}
