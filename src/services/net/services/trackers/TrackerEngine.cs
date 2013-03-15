using System;
using System.Threading;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Nohros.Ruby.Data;
using Nohros.Ruby.Extensions;
using Nohros.Ruby.Protocol;
using Nohros.Ruby.Protocol.Control;
using Nohros.Concurrent;
using R = Nohros.Resources.StringResources;
using ZMQ;
using ZmqSocket = ZMQ.Socket;
using ZmqContext = ZMQ.Context;

namespace Nohros.Ruby
{
  internal class TrackerEngine
  {
    const string kClassName = "Nohros.Ruby.TrackerEngine";
    readonly UdpClient discoverer_;
    readonly RubyLogger logger_;

    readonly IServicesRepository services_repository_;
    readonly ITrackerFactory tracker_factory_;
    readonly Dictionary<Guid, Tracker> trackers_;
    ManualResetEvent broadcast_signaler_;
    Thread broadcast_thread_;
    volatile bool running_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="TrackerEngine"/> class using
    /// the specified <see cref="ZmqContext"/> object.
    /// </summary>
    /// <param name="tracker_factory">
    /// A <see cref="ITrackerFactory"/> object that can be used to create
    /// instances of the <see cref="Tracker"/> class.
    /// </param>
    /// <param name="discoverer">
    /// A <see cref="UdpClient"/> object that can be used to receive beacons
    /// from peers.
    /// </param>
    public TrackerEngine(ITrackerFactory tracker_factory,
      IServicesRepository services_repository, UdpClient discoverer) {
      tracker_factory_ = tracker_factory;
      trackers_ = new Dictionary<Guid, Tracker>();
      services_repository_ = services_repository;
      logger_ = RubyLogger.ForCurrentProcess;
      discoverer_ = discoverer;
      running_ = false;
    }
    #endregion

    public void Start() {
      running_ = true;
      broadcast_signaler_ = new ManualResetEvent(false);
      broadcast_thread_ = new BackgroundThreadFactory()
        .CreateThread(BroadcastThread);
      broadcast_thread_.Start();
    }

    public void Shutdown() {
      Shutdown(Timeout.Infinite);
    }

    public void Shutdown(int timeout) {
      // TODO: Find a way to stop the discoverer thread.
      foreach (var tracker in trackers_) {
        tracker.Value.MessageChannel.Close(timeout);
      }
    }

    /// <summary>
    /// Searches for services that matches the given facts.
    /// </summary>
    /// <param name="facts">
    /// A collection of <see cref="KeyValuePair{TKey,TValue}"/> containing the
    /// facts to search for.
    /// </param>
    /// <param name="callback">
    /// The <see cref="Func{TResult}"/> delegate that should be executed when
    /// the query completes.
    /// </param>
    public void FindServices(IEnumerable<KeyValuePair<string, string>> facts,
      Action<ZMQEndPoint> callback) {
      IEnumerable<ZMQEndPoint> services = services_repository_
        .Query(new ServiceFacts(facts));
      var enumerator = services.GetEnumerator();

      // If a service is not found, put the query in queue.
      if (!enumerator.MoveNext()) {
        // TODO : put the the query in a queue to be processed later.
        return;
      }
      do {
        callback(enumerator.Current);
      } while (enumerator.MoveNext());
    }

    public void OnMessagePacketReceived(RubyMessagePacket packet) {
    }

    public void Announce(IList<KeyValuePair> facts, ZMQEndPoint endpoint) {
      AddService(facts, endpoint);
      BroadcastAnnounce(facts);
    }

    void AddService(IEnumerable<KeyValuePair> facts, ZMQEndPoint endpoint) {
      services_repository_.Add(new ServiceEndpoint {
        Endpoint = endpoint,
        Facts = new ServiceFacts(KeyValuePairs.ToKeyValuePairs(facts))
      });
    }

    void BroadcastAnnounce(IEnumerable<KeyValuePair> facts) {
      AnnounceMessage announcement = new AnnounceMessage.Builder()
        .AddRangeFacts(facts)
        .Build();
      var packet = RubyMessages.CreateMessagePacket(0.AsBytes(),
        (int) NodeMessageType.kNodeAnnounce, announcement.ToByteArray());
      foreach (var tracker in trackers_) {
        tracker.Value.MessageChannel.Send(packet);
      }
    }

    void BroadcastThread() {
      var endpoint = new IPEndPoint(IPAddress.Any, 0);
      while (running_) {
        try {
          byte[] bytes = discoverer_.Receive(ref endpoint);
          if (bytes.Length > 0) {
            var beacon = Beacon.FromByteArray(bytes, endpoint.Address);
            OnBeaconReceived(beacon);
          }
        } catch (SocketException e) {
          // If the socket has been shutdown finish the thread.
          if (e.SocketErrorCode == SocketError.Shutdown) {
            return;
          }
          logger_.Error(string.Format(
            R.Log_MethodThrowsException, "BroadcastThread", kClassName), e);
        } catch (FormatException e) {
          logger_.Error(string.Format(R.Log_MethodThrowsException,
            "BroadcastThread", kClassName), e);
        }
      }
    }

    public void OnBeaconReceived(Beacon beacon) {
      Tracker tracker;
      if (!trackers_.TryGetValue(beacon.PeerID, out tracker) ||
        tracker.MessageChannel.Endpoint != beacon.PeerEndpoint) {
        tracker = tracker_factory_.CreateTracker(beacon.PeerEndpoint,
          Transport.TCP);
        tracker.MessageChannel.Open();
        trackers_[beacon.PeerID] = tracker;

        logger_.Info("discovered to a new tracker at: "
          + tracker.MessageChannel.Endpoint);
      }
      tracker.LastSeen = DateTime.Now;

      if (logger_.IsDebugEnabled) {
        logger_.Debug("beacon received from the tracker associated with the "
          + "ID:  \"" + beacon.PeerID + "\" and located at endpoint: \""
          + beacon.PeerEndpoint.Address + ":" + beacon.PeerEndpoint.Port + "\"");
      }
    }
  }
}
