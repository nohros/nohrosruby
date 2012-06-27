using System;
using Nohros.Concurrent;
using Nohros.Ruby.Protocol;

namespace Nohros.Ruby
{
  /// <summary>
  /// A channel implementation that essentially behaves like "/dev/null". All
  /// calls to Send() will return <c>true</c> although no action is performed.
  /// Added listeners are ignored and the Open method do nothing.
  /// </summary>
  public class NullMessageChannel : IRubyMessageChannel
  {
    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="NullMessageChannel"/>
    /// class.
    /// </summary>
    public NullMessageChannel() {
    }
    #endregion

    /// <inheritdoc/>
    public bool Send(IRubyMessage message, string service) {
      return true;
    }

    /// <inheritdoc/>
    public void AddListener(IRubyMessageListener listener, IExecutor executor,
      string service) {
    }

    /// <inheritdoc/>
    public bool Send(RubyMessagePacket packet) {
      return true;
    }

    /// <inheritdoc/>
    public void Open() {
    }
  }
}
