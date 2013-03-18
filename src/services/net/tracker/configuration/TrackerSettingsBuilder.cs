using System;
using Nohros.Configuration;
using Nohros.Configuration.Builders;

namespace Nohros.Ruby
{
  public partial class TrackerSettings : Configuration.Configuration
  {
    public class Builder : AbstractConfigurationBuilder<TrackerSettings>
    {
      #region .ctor
      public Builder() {
        BroadcastPort = 8520;
      }
      #endregion

      public override TrackerSettings Build() {
        return new TrackerSettings(this);
      }

      public void SetBroadcastPort(int port) {
        BroadcastPort = port;
      }

      public int BroadcastPort { get; private set; }
    }
  }
}
