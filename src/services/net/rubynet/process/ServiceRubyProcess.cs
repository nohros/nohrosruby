using System;
using Nohros.Ruby.Protocol;

namespace Nohros.Ruby
{
  /// <summary>
  /// A <see cref="IRubyProcess"/> that runs as a pseudo windows service.
  /// </summary>
  internal class ServiceRubyProcess : AbstractRubyProcess, IRubyMessagePacketListener
  {
    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceRubyProcess"/>
    /// class.
    /// </summary>
    public ServiceRubyProcess(IRubySettings settings,
      IRubyMessageChannel ruby_message_channel)
      : base(settings, ruby_message_channel) {
    }
    #endregion

    /// <inheritdoc/>
    void IRubyMessagePacketListener.OnMessagePacketReceived(RubyMessagePacket packet) {
    }
  }
}
