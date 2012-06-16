using System;

namespace Nohros.Ruby
{
  /// <summary>
  /// A <see cref="IRubyProcess"/> that runs as a pseudo windows service.
  /// </summary>
  internal class ServiceRubyProcess : IRubyProcess
  {
    readonly IRubyMessageSender message_sender_;
    readonly IPCChannel ipc_channel_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceRubyProcess"/>
    /// class.
    /// </summary>
    /// <param name="ipc_channel">
    /// A <see cref="IPCChannel"/> object that is used to handle the
    /// communication with the external world.
    /// </param>
    public ServiceRubyProcess(IPCChannel ipc_channel) {
      ipc_channel_ = ipc_channel;
    }
    #endregion

    public void Run() {
      throw new NotImplementedException();
    }

    public void Run(string command_line_string) {
      throw new NotImplementedException();
    }

    public IRubyMessageSender MessageSender {
      get { throw new NotImplementedException(); }
    }

    /// <inheritdoc/>
    public IPCChannel IPCChannel {
      get { return ipc_channel_; }
    }
  }
}
