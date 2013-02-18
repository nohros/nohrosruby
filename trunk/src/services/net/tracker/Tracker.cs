using System;
using System.Threading;
using Nohros.Ruby.Protocol;

namespace Nohros.Ruby
{
  public class Tracker : AbstractRubyService
  {
    readonly ManualResetEvent start_stop_event_;
    Thread start_thread_;

    #region .ctor
    public Tracker() {
      start_stop_event_ = new ManualResetEvent(false);
    }
    #endregion

    public override void Start(IRubyServiceHost service_host) {
      start_thread_ = Thread.CurrentThread;
      start_stop_event_.WaitOne();
    }

    public override void Stop(IRubyMessage message) {
      start_stop_event_.Set();
      start_thread_.Join();
    }

    public override void OnMessage(IRubyMessage message) {
    }
  }
}
