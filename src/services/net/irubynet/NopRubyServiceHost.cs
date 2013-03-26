using System;
using System.Collections.Generic;
using Nohros.Concurrent;
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
      Service = new NopService();
    }
    #endregion

    public IRubyService Service { get; private set; }

    public void Shutdown() {
    }

    public void Announce(IDictionary<string, string> facts) {
    }

    public void AddListener(IRubyMessageListener listener, IExecutor executor) {
    }

    /// <inheritdoc/>
    public bool Send(IRubyMessage message) {
      return true;
    }

    /// <inheritdoc/>
    public bool Send(IRubyMessage message,
      IEnumerable<KeyValuePair<string, string>> facts) {
      return true;
    }

    /// <inheritdoc/>
    public bool Send(byte[] message_id, int type, byte[] message) {
      return true;
    }

    /// <inheritdoc/>
    public bool Send(byte[] message_id, int type, byte[] message, string token) {
      return true;
    }

    /// <inheritdoc/>
    public bool Send(byte[] message_id, int type, byte[] message,
      byte[] destination) {
      return true;
    }

    /// <inheritdoc/>
    public bool Send(byte[] message_id, int type, byte[] message, string token,
      byte[] destination) {
      return true;
    }

    /// <inheritdoc/>
    public bool Send(byte[] message_id, int type, byte[] message,
      IEnumerable<KeyValuePair<string, string>> facts) {
      return true;
    }

    /// <inheritdoc/>
    public bool Send(byte[] message_id, int type, byte[] message, string token,
      IEnumerable<KeyValuePair<string, string>> facts) {
      return true;
    }

    /// <inheritdoc/>
    public bool Send(byte[] message_id, int type, byte[] message,
      byte[] destination, IEnumerable<KeyValuePair<string, string>> facts) {
      return true;
    }

    /// <inheritdoc/>
    public bool Send(byte[] message_id, int type, byte[] message, string token,
      byte[] destination, IEnumerable<KeyValuePair<string, string>> facts) {
      return true;
    }

    /// <inheritdoc/>
    public IRubyLogger Logger {
      get { return logger_; }
    }
  }
}
