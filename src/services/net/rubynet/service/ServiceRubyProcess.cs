using System;
using Nohros.Ruby.Protocol;

namespace Nohros.Ruby
{
  /// <summary>
  /// A <see cref="IRubyProcess"/> that runs as a pseudo windows service.
  /// </summary>
  internal class ServiceRubyProcess : AbstractRubyProcess, IRubyMessageListener
  {
    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceRubyProcess"/>
    /// class.
    /// </summary>
    /// <param name="ruby_message_channel">
    /// A <see cref="RubyMessageChannel"/> object that is used to handle the
    /// communication with the external world.
    /// </param>
    public ServiceRubyProcess(IRubySettings settings,
      IRubyMessageChannel ruby_message_channel)
      : base(settings, ruby_message_channel) {
    }
    #endregion

    /// <inheritdoc/>
    void IRubyMessageListener.OnMessagePacketReceived(RubyMessagePacket packet) {
    }
  }
}
