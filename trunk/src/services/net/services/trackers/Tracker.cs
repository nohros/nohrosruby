using System;

namespace Nohros.Ruby
{
  /// <summary>
  /// Tracks the location of services.
  /// </summary>
  public class Tracker
  {
    #region .ctor
    /// <summary>
    /// Initializes  a new instance of the <see cref="Tracker"/> class using
    /// the specified <see cref="TrackerMessageChannel"/> object.
    /// </summary>
    /// <param name="channel"></param>
    public Tracker(TrackerMessageChannel channel) {
      MessageChannel = channel;
    }
    #endregion

    /// <summary>
    /// Gets or sest the <see cref="TrackerMessageChannel"/> that is used
    /// to send messages to a tracker.
    /// </summary>
    public TrackerMessageChannel MessageChannel { get; set; }

    /// <summary>
    /// Gets or sets the date and time the tracker was last seen.
    /// </summary>
    /// <remarks>
    /// A tracker sends a beacon at regular intervals to mark its presence.
    /// This property register the date and time when the last beacon was
    /// received by a tracker.
    /// </remarks>
    public DateTime LastSeen { get; set; }
  }
}
