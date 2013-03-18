using System;
using Nohros.Configuration;

namespace Nohros.Ruby
{
  public partial class TrackerSettings
  {
    public class Loader : AbstractConfigurationLoader<TrackerSettings>
    {
      #region .ctor
      public Loader() : base(new Builder()) {
      }
      #endregion
    }
  }
}
