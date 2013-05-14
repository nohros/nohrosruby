using System;
using System.Diagnostics;
using System.Threading;
using Nohros.Ruby.Protocol;
using Nohros.Ruby.Protocol.Control;

namespace Nohros.Ruby
{
  internal class ServiceSelfHostProcess : SelfHostProcess
  {
    readonly RubyLogger logger_;
    readonly ManualResetEvent start_stop_event_;
    Thread run_thread_;

    #region .ctor
    public ServiceSelfHostProcess(RubySettings settings,
      HostMessageChannel host_message_channel, TrackerEngine trackers)
      : base(settings, host_message_channel, trackers) {
      logger_ = RubyLogger.ForCurrentProcess;
      start_stop_event_ = new ManualResetEvent(false);
    }
    #endregion

    public override void Run(string command_line_string) {
      base.Run(command_line_string);
      run_thread_ = Thread.CurrentThread;
      start_stop_event_.WaitOne();
    }

    protected override void StartService(ServiceControlMessage message) {
      var own_process = Process.GetCurrentProcess();
      var new_process = new Process();
      ProcessStartInfo info = new_process.StartInfo;
      info.FileName = own_process.MainModule.FileName;
      info.CreateNoWindow = true;
      info.Arguments = "--debug";
      new_process.Start();
    }

    public override void OnMessagePacketReceived(RubyMessagePacket packet) {
      base.OnMessagePacketReceived(packet);
    }

    public override void Exit() {
      base.Exit();
      start_stop_event_.Set();

      if (run_thread_ != null) {
        run_thread_.Join(20000);
      }
    }
  }
}
