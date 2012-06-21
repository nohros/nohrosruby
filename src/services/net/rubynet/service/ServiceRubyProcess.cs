using System;
using Nohros.Concurrent;
using Nohros.Ruby.Protocol;

namespace Nohros.Ruby
{
  /// <summary>
  /// A <see cref="IRubyProcess"/> that runs as a pseudo windows service.
  /// </summary>
  internal class ServiceRubyProcess : AbstractRubyProcess, IRubyProcess,
                                      IRubyMessageListener
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
    public ServiceRubyProcess(IRubyMessageChannel ruby_message_channel)
      : base(ruby_message_channel) {
    }
    #endregion

    /// <inheritdoc/>
    void IRubyMessageListener.OnMessagePacketReceived(RubyMessagePacket packet) {
    }

    public override void Run(string command_line_string) {
      RubyMessageChannel.AddListener(this, Executors.SameThreadExecutor());
    }
  }
}
