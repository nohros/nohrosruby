using System;
using System.Collections.Generic;
using Nohros.Ruby.Data;
using Nohros.Ruby.Extensions;
using Nohros.Ruby.Protocol.Control;
using ZMQ;
using ZmqSocket = ZMQ.Socket;
using ZmqContext = ZMQ.Context;

namespace Nohros.Ruby
{
  internal class Trackers
  {
    readonly IServicesRepository services_repository_;
    readonly ITrackerFactory tracker_factory_;
    readonly Dictionary<Guid, Tracker> trackers_;
    readonly RubyLogger logger_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="Trackers"/> class using
    /// the specified <see cref="ZmqContext"/> object.
    /// </summary>
    /// <param name="tracker_factory">
    /// A <see cref="ITrackerFactory"/> object that can be used to create
    /// instances of the <see cref="Tracker"/> class.
    /// </param>
    public Trackers(ITrackerFactory tracker_factory,
      IServicesRepository services_repository) {
      tracker_factory_ = tracker_factory;
      trackers_ = new Dictionary<Guid, Tracker>();
      services_repository_ = services_repository;
      logger_ = RubyLogger.ForCurrentProcess;
    }
    #endregion

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

    public void OnServiceAnnounce(AnnounceMessage message, ZMQEndPoint endpoint) {
      services_repository_.Add(new ServiceEndpoint {
        Endpoint = endpoint,
        Facts = new ServiceFacts(KeyValuePairs.ToKeyValuePairs(
          message.FactsList))
      });
      BroadcastAnnounce(message);
    }

    void BroadcastAnnounce(AnnounceMessage message) {
      foreach (var tracker in trackers_) {
        var packet = RubyMessages.CreateMessagePacket(0.AsBytes(),
          (int) NodeMessageType.kNodeAnnounce, message.ToByteArray());
        tracker.Value.MessageChannel.Send(packet);
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
      }
      tracker.LastSeen = DateTime.Now;

      if (logger_.IsDebugEnabled) {
        logger_.Debug("Beacon received from the tracker associated with the "
          + "ID:  \"" + beacon.PeerID + "\" and located at endpoint: \""
          + beacon.PeerEndpoint.Address + ":" + beacon.PeerEndpoint.Port + "\"");
      }
    }
  }
}
