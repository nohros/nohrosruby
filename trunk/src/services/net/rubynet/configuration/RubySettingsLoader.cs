using System;
using System.Globalization;
using System.Xml;
using Nohros.Configuration;
using Nohros.Extensions;
using Nohros.Logging;

namespace Nohros.Ruby
{
  internal partial class RubySettings
  {
    public class Loader : AbstractConfigurationLoader<RubySettings>
    {
      readonly Builder builder_;

      #region .ctor
      /// <summary>
      /// Initializes a new instance of the <see cref="Loader"/> class using
      /// the specified <see cref="Builder"/>.
      /// </summary>
      /// <param name="builder">
      /// A <see cref="Builder"/> object containing pre configured values.
      /// </param>
      public Loader(Builder builder)
        : base(builder) {
        builder_ = builder;
        RunningMode = "normal";
      }

      public Loader()
        : this(new Builder()) {
      }
      #endregion

      public override RubySettings CreateConfiguration(
        IConfigurationBuilder<RubySettings> builder) {
        builder_.SetRunningMode(GetRunningMode());
        builder_.SetCulture(GetCultureInfo());
        return base.CreateConfiguration(builder);
      }


      CultureInfo GetCultureInfo() {
        try {
          return new CultureInfo(Culture);
        } catch {
          return CultureInfo.CurrentCulture;
        }
      }

      RunningMode GetRunningMode() {
        if (RunningMode.CompareOrdinalIgnoreCase(Strings.kInteractiveRunningMode)) {
          return Ruby.RunningMode.Interactive;
        }
        return Ruby.RunningMode.Service;
      }

      public string RunningMode { get; set; }
      public string Culture { get; set; }
    }
  }
}
