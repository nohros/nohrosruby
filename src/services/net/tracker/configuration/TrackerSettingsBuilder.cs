using System;
using Nohros.Configuration;
using Nohros.Configuration.Builders;

namespace Nohros.Ruby
{
  public partial class TrackerSettings : Configuration.Configuration
  {
    public class Builder : AbstractConfigurationBuilder<TrackerSettings>
    {
      public override TrackerSettings Build() {
        return new TrackerSettings(this);
      }
    }
  }
}
