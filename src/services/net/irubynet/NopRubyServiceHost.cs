using System;
using System.Collections.Generic;
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

    public bool Send(IRubyMessage message,
      IEnumerable<KeyValuePair<string, string>> facts) {
      return true;
    }

    public bool Send(byte[] message_id, int type, byte[] message,
      byte[] destination, IEnumerable<KeyValuePair<string, string>> facts) {
      return true;
    }

    public bool Send(byte[] message_id, int type, byte[] message,
      byte[] destination, string token,
      IEnumerable<KeyValuePair<string, string>> facts) {
      return true;
    }

    public byte[] FormatErrorMessage(byte[] message_id, int exception_code,
      string error, byte[] destination) {
      return new byte[0];
    }

    public byte[] FormatErrorMessage(byte[] message_id, int exception_code,
      string error, byte[] destination, Exception exception) {
      return new byte[0];
    }

    public byte[] FormatErrorMessage(byte[] message_id, int exception_code,
      byte[] destination, Exception exception) {
      return new byte[0];
    }

    public bool Send(byte[] message_id, int type, byte[] message,
      byte[] destination, string token) {
      return true;
    }
  }
}
