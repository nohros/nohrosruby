using System;
using ZMQ;

namespace Nohros.Ruby.process
{
  internal abstract class AbstractSelfHostProcess : AbstractRubyProcess
  {
    readonly Context context_;
    readonly Socket receiver_;
    readonly Socket sender_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="AbstractSelfHostProcess"/>
    /// using the specified application settings and IPC communication channel.
    /// </summary>
    /// <param name="settings">
    /// A <see cref="IRubySettings"/> containing the user defined application
    /// settings.
    /// </param>
    /// <param name="ruby_message_channel">
    /// A <see cref="IRubyMessageChannel"/> that provides a link between the
    /// internal and external processes.
    /// </param>
    /// <param name="context">
    /// </param>
    protected AbstractSelfHostProcess(IRubySettings settings,
      IRubyMessageChannel ruby_message_channel, Context context)
      : base(settings, ruby_message_channel) {
      context_ = context;
      receiver_ = context_.Socket(SocketType.ROUTER);
      sender_ = context_.Socket(SocketType.DEALER);
    }
    #endregion

    /// <inheritdoc/>
    public override void Run(string command_line_string) {
      base.Run(command_line_string);
    }
  }
}
