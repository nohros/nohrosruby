using System;
using Nohros.Configuration;

namespace Nohros.Ruby
{
  public partial class TrackerSettings : ITrackerSettings, IConfiguration
  {
    readonly Builder builder_;

    #region .ctor
    public TrackerSettings(Builder builder) : base(builder) {
      builder_ = builder;
    }
    #endregion
  }
}
