using System;
using Nohros.Ruby;
using Nohros.Ruby.Protocol;

namespace Nohros.Ruby
{
  internal class ShellSelfHostProcess : IRubyProcess
  {
    readonly SelfHostProcess self_host_process_;
    readonly ShellRubyProcess shell_ruby_process_;

    public void OnMessagePacketReceived(RubyMessagePacket packet) {
      shell_ruby_process_.OnMessagePacketReceived(packet);
    }

    #region .ctor
    public ShellSelfHostProcess(ShellRubyProcess shell_ruby_process,
      SelfHostProcess self_host_process) {
      shell_ruby_process_ = shell_ruby_process;
      self_host_process_ = self_host_process;
    }
    #endregion

    public void Run() {
      Run(string.Empty);
    }

    public void Run(string command_line_string) {
      self_host_process_.Run(command_line_string);
      shell_ruby_process_.Run(command_line_string);
      Exit();
    }

    public void Exit() {
      self_host_process_.Exit();
    }
  }
}
