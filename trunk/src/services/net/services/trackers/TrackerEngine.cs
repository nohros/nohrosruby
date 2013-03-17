using System;
using System.Threading;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Nohros.Ruby.Data;
using Nohros.Ruby.Extensions;
using Nohros.Ruby.Protocol;
using Nohros.Ruby.Protocol.Control;
using R = Nohros.Resources.StringResources;
using ZMQ;
using ZmqSocket = ZMQ.Socket;
using ZmqContext = ZMQ.Context;

namespace Nohros.Ruby
{
  internal class TrackerEngine
  {
    const string kClassName = "Nohros.Ruby.TrackerEngine";
    const string kQueryServiceToken = "query-service-token";

    readonly UdpClient discoverer_;
    readonly RubyLogger logger_;
    readonly Dictionary<int, Action<ZMQEndPoint>> queries_;
    readonly Dictionary<string, Action<RubyMessage>> reseponse_handlers_;

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
      queries_ = new Dictionary<int, Action<ZMQEndPoint>>();

      reseponse_handlers_ = new Dictionary<string, Action<RubyMessage>> {
        {kQueryServiceToken, OnQueryServiceResponse}
      };
    }
    #endregion

    public void Start() {
      running_ = true;
      discoverer_.BeginReceive(ReceiveBeacon, null);
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
      IServicesByFactsQuery q;
      IEnumerable<ZMQEndPoint> services = services_repository_
        .Query(out q)
        .SetFacts(new ServiceFacts(facts))
        .Execute();

      var enumerator = services.GetEnumerator();
      if (enumerator.MoveNext()) {
        do {
          callback(enumerator.Current);
        } while (enumerator.MoveNext());
        return;
      }

      // If a service is not found in the local queue, ask the connected
      // trackers about the service.
      var query = new QueryMessage.Builder()
        .AddRangeFacts(KeyValuePairs.FromKeyValuePairs(facts))
        .Build();

      // Enqueue the query, so we can call the callback handler when the
      // query is resolved. We use the delegate hashcode as the key to avoid
      // override a query with query that is already running.
      int key = callback.GetHashCode();

      // Locking the "queries_" object here is fine, since this is not
      // exposed to outside.
      lock (queries_) {
        queries_[key] = callback;
      }

      // Send the message to all the connected trackers. Response duplication
      // should be handled at response time.
      foreach (var tracker in trackers_) {
        var channel = tracker.Value.MessageChannel;
        var packet = RubyMessages.CreateMessagePacket(key.AsBytes(),
          (int) NodeMessageType.kNodeQuery, query.ToByteArray(),
          kQueryServiceToken);
        channel.Send(packet);
      }
    }

    public void OnMessagePacketReceived(RubyMessagePacket packet) {
      RubyMessage message = packet.Message;
      switch (message.Type) {
        case (int) NodeMessageType.kNodeResponse:
          OnResponse(message);
          break;
      }
    }

    void OnResponse(RubyMessage message) {
      Action<RubyMessage> handler;
      if (reseponse_handlers_.TryGetValue(message.Token, out handler)) {
        handler(message);
      }
    }

    void OnQueryServiceResponse(RubyMessage message) {
      try {
        int id = message.Id.AsInt();
        ResponseMessage response = ResponseMessage.ParseFrom(message.Message);
        Action<ZMQEndPoint> callback;
        if (queries_.TryGetValue(id, out callback)) {
          lock (queries_) {
            // If the callback does not exists anymore, someone is already
            // processing the response.
            if (!queries_.Remove(id)) {
              return;
            }
          }
          foreach (var pair in response.ReponsesList) {
            try {
              callback(new ZMQEndPoint(pair.Value));
            } catch (ArgumentException ae) {
              logger_.Error(
                string.Format(Resources.ZMQEndpoint_InvalidFormat, pair.Key,
                  pair.Value), ae);
            }
          }
        }
      } catch (System.Exception e) {
        logger_.Error(
          string.Format(R.Log_MethodThrowsException, "OnQueryServiceResponse",
            kClassName), e);
      }
    }

    public void Announce(IList<KeyValuePair> facts, ZMQEndPoint endpoint) {
      AddService(facts, endpoint);
      BroadcastAnnounce(facts);
    }

    void AddService(IEnumerable<KeyValuePair> facts, ZMQEndPoint endpoint) {
      INewServiceCommand command;
      services_repository_
        .Query(out command)
        .SetEndPoint(endpoint)
        .SetFacts(new ServiceFacts(KeyValuePairs.ToKeyValuePairs(facts)));
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

    void ReceiveBeacon(IAsyncResult result) {
      try {
        var endpoint = new IPEndPoint(IPAddress.Any, 0);
        var bytes = discoverer_.EndReceive(result, ref endpoint);
        if (bytes.Length > 0) {
          var beacon = Beacon.FromByteArray(bytes, endpoint.Address);
          OnBeaconReceived(beacon);
          if (running_) {
            discoverer_.BeginReceive(ReceiveBeacon, null);
          }
        }
      } catch (SocketException e) {
        // If the socket has been shutdown finish the thread.
        if (e.SocketErrorCode == SocketError.Shutdown) {
          return;
        }
        logger_.Error(string.Format(
          R.Log_MethodThrowsException, "Broadcast", kClassName), e);
      } catch (FormatException e) {
        logger_.Error(string.Format(R.Log_MethodThrowsException,
          "Broadcast", kClassName), e);
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
