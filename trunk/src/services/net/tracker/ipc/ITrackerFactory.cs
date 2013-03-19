using System;
using System.Net;
using ZMQ;

namespace Nohros.Ruby
{
  /// <summary>
  /// A factory to created instances of the <see cref="TrackerMessageChannel"/>
  /// class.
  /// </summary>
  public interface ITrackerFactory
  {
    /// <summary>
    /// Creates a new instance of the <see cref="TrackerMessageChannel"/> class
    /// for the specified <see cref="IPEndPoint"/> and <see cref="Transport"/>.
    /// </summary>
    /// <param name="endpoint">
    /// The IP address of the channel.
    /// </param>
    /// <param name="transport">
    /// The transport protocol that should be used by the channel.
    /// </param>
    /// <returns></returns>
    Tracker CreateTracker(IPEndPoint endpoint, Transport transport,
      byte[] peer_id);
  }
}
