using System;
using Nohros.Configuration;

namespace Nohros.Ruby.Logging
{
  public partial class Settings
  {
    public class Loader : AbstractConfigurationLoader<Settings>
    {
      #region .ctor
      public Loader() : this(new Builder()) {
      }

      public Loader(IConfigurationBuilder<Settings> builder) : base(builder) {
      }
      #endregion
    }
  }
}
