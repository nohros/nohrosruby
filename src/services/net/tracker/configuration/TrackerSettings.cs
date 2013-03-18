using System;
using Nohros.Configuration;

namespace Nohros.Ruby
{
  public partial class TrackerSettings : ITrackerSettings
  {
    readonly Builder builder_;

    #region .ctor
    public TrackerSettings(Builder builder) : base(builder) {
      builder_ = builder;
      BroadcastPort = builder.BroadcastPort;
    }
    #endregion

    public int BroadcastPort { get; private set; }
  }
}
