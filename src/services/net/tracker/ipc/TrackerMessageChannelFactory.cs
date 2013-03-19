using System;
using System.Net;
using ZMQ;
using ZmqContext = ZMQ.Context;

namespace Nohros.Ruby
{
  /// <summary>
  /// A factory calls for tracking related classes.
  /// </summary>
  public class TrackerMessageChannelFactory
  {
    readonly ZmqContext context_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="TrackerMessageChannelFactory"/> class using the specified
    /// context.
    /// </summary>
    /// <param name="context">
    /// A <see cref="Context"/> that can be used to create instances of the
    /// <see cref="Socket"/> object.
    /// </param>
    public TrackerMessageChannelFactory(ZmqContext context) {
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
    public TrackerMessageChannel CreateTrackerMessageChannel(
      IPEndPoint endpoint, Transport transport, byte[] peer_id) {
      return new TrackerMessageChannel(context_,
        new ZMQEndPoint(endpoint, transport), peer_id);
    }
  }
}
