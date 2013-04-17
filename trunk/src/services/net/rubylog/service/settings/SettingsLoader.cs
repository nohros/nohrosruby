using System;
using Nohros.Configuration;

namespace Nohros.Ruby.Logging
{
  public partial class Settings
  {
    public class Loader : AbstractConfigurationLoader<Settings>
    {
      #region .ctor
      public Loader() : base(new Builder()) {
      }
      #endregion
    }
  }
}
