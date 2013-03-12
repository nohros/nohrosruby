using System;
using System.Net;
using ZMQ;
using ZmqContext = ZMQ.Context;
using ZmqSocket = ZMQ.Socket;

namespace Nohros.Ruby
{
  public class TrackerFactory : ITrackerFactory
  {
    readonly ZmqContext context_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="TrackerFactory"/> class
    /// using the specified <see cref="ZmqContext"/> object.
    /// </summary>
    /// <param name="context">
    /// A <see cref="ZmqContext"/> that can be used to create instances of the
    /// <see cref="ZmqSocket"/>.
    /// </param>
    public TrackerFactory(ZmqContext context) {
      context_ = context;
    }
    #endregion

    /// <summary>
    /// Creates a new instance of the <see cref="TrackerMessageChannel"/>
    /// using the specified endpoint and transport protocol.
    /// </summary>
    /// <param name="endpoint"></param>
    /// <param name="transport"></param>
    /// <returns></returns>
    public Tracker CreateTracker(IPEndPoint endpoint, Transport transport) {
      var zmq_endpoint = new ZMQEndPoint(endpoint, transport);
      var channel = new TrackerMessageChannel(context_, zmq_endpoint);
      var tracker = new Tracker(channel);
      return tracker;
    }
  }
}
