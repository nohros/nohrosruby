using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Nohros.Ruby.Data;
using Nohros.Ruby.Extensions;
using Nohros.Ruby.Protocol;
using Nohros.Ruby.Protocol.Control;
using ZMQ;
using Exception = System.Exception;
using R = Nohros.Resources.StringResources;

namespace Nohros.Ruby
{
  public class TrackerService : AbstractRubyService
  {
    const string kClassName = "Nohros.Ruby.TrackerService";

    readonly Broadcaster broadcaster_;
    readonly RubyLogger logger_;
    readonly Dictionary<string, Tracker> nodes_;
    readonly IServicesRepository services_repository_;
    readonly ManualResetEvent start_stop_event_;
    readonly ITrackerFactory tracker_factory_;
    Thread start_thread_;

    #region .ctor
    public TrackerService(IServicesRepository services_repository,
      Broadcaster broadcaster, ITrackerFactory tracker_factory) {
      start_stop_event_ = new ManualResetEvent(false);
      services_repository_ = services_repository;
      broadcaster_ = broadcaster;
      logger_ = RubyLogger.ForCurrentProcess;
      tracker_factory_ = tracker_factory;
      nodes_ = new Dictionary<string, Tracker>();
    }
    #endregion

    public override void Start(IRubyServiceHost service_host) {
      base.Start(service_host);
      start_thread_ = Thread.CurrentThread;
      start_stop_event_.WaitOne();
    }

    public override void Stop(IRubyMessage message) {
      start_stop_event_.Set();
      broadcaster_.Stop();
      start_thread_.Join();
    }

    public override void OnMessage(IRubyMessage message) {
      IMessageHandler handler;
      switch (message.Type) {
        case (int) NodeMessageType.kNodeHello:
          handler = new HelloMessageHandler(nodes_, broadcaster_,
            tracker_factory_);
          break;

        case (int) NodeMessageType.kNodeAnnounce:
          handler = new AnnounceMessageHandler(nodes_, services_repository_);
          break;

        default:
          return;
      }
      handler.Handle(message);
    }
  }
}
