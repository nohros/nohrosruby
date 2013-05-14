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
  internal delegate void TrackerDiscoveredEventHandler(Tracker tracker);

  internal class TrackerEngine
  {
    const string kClassName = "Nohros.Ruby.TrackerEngine";
    const string kQueryServiceToken = "query-service-token";

    readonly Broadcaster broadcaster_;
    readonly RubyLogger logger_;
    readonly Queue<RubyMessagePacket> pending_queries_;
    readonly Dictionary<int, Action<ZMQEndPoint>> queries_;
    readonly Dictionary<string, Action<RubyMessage>> reseponse_handlers_;

    readonly IServicesRepository services_repository_;
    readonly ITrackerFactory tracker_factory_;
    readonly Dictionary<string, Tracker> trackers_;

    readonly QueryMessageHandler query_message_handler_;
    ZMQEndPoint endpoint_;

    volatile byte[] peer_id_;
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
    public TrackerEngine(ITrackerFactory tracker_factory,
      IServicesRepository services_repository, Broadcaster broadcaster) {
      tracker_factory_ = tracker_factory;
      trackers_ = new Dictionary<string, Tracker>();
      services_repository_ = services_repository;
      logger_ = RubyLogger.ForCurrentProcess;
      broadcaster_ = broadcaster;
      queries_ = new Dictionary<int, Action<ZMQEndPoint>>();
      pending_queries_ = new Queue<RubyMessagePacket>();
      peer_id_ = Guid.NewGuid().ToByteArray();
      broadcaster_.BeaconReceived += OnBeaconReceived;
      running_ = false;
      EnableTracker = false;

      reseponse_handlers_ = new Dictionary<string, Action<RubyMessage>> {
        {kQueryServiceToken, OnQueryServiceResponse}
      };
    }
    #endregion

    public void OnTrackerDiscovered(Tracker tracker) {
      HelloMessage hello = CreateHelloMessage();
      RubyMessagePacket packet = RubyMessages.CreateMessagePacket(0.AsBytes(),
        (int) NodeMessageType.kNodeHello, hello.ToByteArray());
      tracker.MessageChannel.Send(packet);

      // send the pending queries to the new discovered tracker, so the
      // queries that was performed when we did not know about any tracker.
      lock (pending_queries_) {
        var channel = tracker.MessageChannel;
        while (pending_queries_.Count > 0) {
          channel.Send(pending_queries_.Dequeue());
        }
      }

      Listeners.SafeInvoke<TrackerDiscoveredEventHandler>(TrackerDiscovered,
        handler => handler(tracker));
    }

    HelloMessage CreateHelloMessage() {
      // If the channel is bound to all network interfaces use the first
      // one as the mailbox.
      return new HelloMessage.Builder()
        .SetAddress(endpoint_.Address)
        .SetPort(endpoint_.Port)
        .Build();
    }

    /// <summary>
    /// Starts listening for beacons.
    /// </summary>
    /// <remarks>
    /// This overloads starts listening for incoming beacons only, if you want
    /// to starts broadcasting beacons, you should call the
    /// <see cref="Start"/> overload.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// The service was already started.
    /// </exception>
    public void Start(ZMQEndPoint endpoint) {
      if (running_) {
        throw new InvalidOperationException("The service is already started");
      }

      if (endpoint.Address == "*") {
        var addresses = Dns.GetHostAddresses(string.Empty);
        foreach (var ip in addresses) {
          if (ip.AddressFamily == AddressFamily.InterNetwork) {
            endpoint_ = new ZMQEndPoint(ip.ToString(), endpoint.Port,
              endpoint.Transport);
            break;
          }
        }
      } else {
        endpoint_ = endpoint;
      }

      broadcaster_.Listen();
      if (EnableTracker) {
        broadcaster_.Broadcast(peer_id_, endpoint_.Port);
      }
    }


    public void Shutdown() {
      Shutdown(Timeout.Infinite);
    }

    public void Shutdown(int timeout) {
      // TODO: Find a way to stop the discoverer thread.
      foreach (var tracker in trackers_) {
        tracker.Value.MessageChannel.Close(timeout);
      }
      broadcaster_.Stop();
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

      // If a service is not found in the local database, ask the connected
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
      //
      // If there are no trackers enqueue the query and send it to the first
      // discovered tracker.
      var packet = RubyMessages.CreateMessagePacket(key.AsBytes(),
        (int) NodeMessageType.kNodeQuery, query.ToByteArray(),
        kQueryServiceToken);
      if (trackers_.Count == 0) {
        lock (pending_queries_) {
          pending_queries_.Enqueue(packet);
        }
      } else {
        foreach (var tracker in trackers_) {
          var channel = tracker.Value.MessageChannel;
          channel.Send(packet);
        }
      }
    }

    public void OnMessagePacketReceived(RubyMessagePacket packet) {
      RubyMessage message = packet.Message;
      switch (message.Type) {
        case (int) NodeMessageType.kNodeResponse:
          OnResponse(message);
          break;
        case (int) NodeMessageType.kNodeHello:
          new HelloMessageHandler(trackers_, broadcaster_, tracker_factory_)
            .Handle(message);
          break;

        case (int) NodeMessageType.kNodeAnnounce:
          new AnnounceMessageHandler(trackers_, services_repository_)
            .Handle(message);
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
      new QueryResponseMessageHandler(queries_).Handle(message);
    }

    /// <summary>
    /// Send an announcement message to the given tracker.
    /// </summary>
    /// <param name="facts">
    /// The facts associated with the service that is sending the announcement.
    /// </param>
    /// <param name="tracker">
    /// The <see cref="Tracker"/> that should receive the annouce message.
    /// </param>
    public void Announce(IEnumerable<KeyValuePair> facts, Tracker tracker) {
      AnnounceMessage announcement = new AnnounceMessage.Builder()
        .AddRangeFacts(facts)
        .Build();
      var packet = RubyMessages.CreateMessagePacket(0.AsBytes(),
        (int) NodeMessageType.kNodeAnnounce, announcement.ToByteArray());
      tracker.MessageChannel.Send(packet);
    }

    /// <summary>
    /// Send an announce message to all connected trackers and store the .
    /// </summary>
    /// <param name="facts">
    /// The facts associated with the service that is sending the announcement.
    /// </param>
    public void Announce(IList<KeyValuePair> facts) {
      foreach (var tracker in trackers_.Values) {
        Announce(facts, tracker);
      }
    }

    void AddService(IEnumerable<KeyValuePair> facts, ZMQEndPoint endpoint) {
      // TODO: Avail the performance gain in keep a local service database
      // and consider removing this function if the performance gain is
      // not considerable.
      INewServiceCommand command;
      services_repository_
        .Query(out command)
        .SetEndPoint(endpoint)
        .SetFacts(new ServiceFacts(KeyValuePairs.ToKeyValuePairs(facts)));
    }

    void OnBeaconReceived(Beacon beacon) {
      // Ignore beacons that was sent from us.
      if (beacon.PeerID.AsBase64() == peer_id_.AsBase64()) {
        return;
      }

      if (logger_.IsDebugEnabled) {
        logger_.Debug(
          "beacon received from the tracker associated with the ID: \"" +
            beacon.PeerID.AsBase64() + "\" and located at endpoint: \""
            + beacon.PeerEndpoint.Address + ":" + beacon.PeerEndpoint.Port +
            "\"");
      }

      Tracker tracker;
      if (!trackers_.TryGetValue(beacon.PeerID.AsBase64(), out tracker) ||
        tracker.MessageChannel.Endpoint != beacon.PeerEndpoint) {
        tracker = tracker_factory_.CreateTracker(beacon.PeerEndpoint,
          Transport.TCP, peer_id_);
        tracker.MessageChannel.Open();
        trackers_[beacon.PeerID.AsBase64()] = tracker;

        logger_.Info("discovered a new tracker at: "
          + tracker.MessageChannel.Endpoint);

        OnTrackerDiscovered(tracker);
      }
      tracker.LastSeen = DateTime.Now;
    }

    public event TrackerDiscoveredEventHandler TrackerDiscovered;

    /// <summary>
    /// Gets or sets the ID of the peer that is used to identify the local
    /// service node.
    /// </summary>
    /// <remarks>
    /// When self host mode is used we need to set this field with the ID of
    /// the local peer to avoid making connection to ourselves.
    /// </remarks>
    public bool EnableTracker { get; set; }
  }
}
