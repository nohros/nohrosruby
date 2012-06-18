using System;
using System.Threading;
using Nohros.Concurrent;

namespace Nohros.Ruby
{
  /// <summary>
  /// A <see cref="IRubyProcess"/> that runs as a pseudo windows service.
  /// </summary>
  internal class ServiceRubyProcess : IRubyProcess, IRubyMessageListener {
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
      Run(string.Empty);
    }

    public void Run(string command_line_string) {
      ipc_channel_.AddListener(this, Executors.SameThreadExecutor());
    }

    /// <inheritdoc/>
    void IRubyMessageListener.OnMessagePacketReceived(RubyMessagePacket packet) {
    }

    /// <inheritdoc/>
    string[] IRubyMessageListener.Filters {
      get { return new string[] { "RSH" }; }
    }

    /// <inheritdoc/>
    public IPCChannel IPCChannel {
      get { return ipc_channel_; }
    }
  }
}
