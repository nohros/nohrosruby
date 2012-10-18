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
      #region .ctor
      /// <summary>
      /// Initializes a new instance of the <see cref="Loader"/> class using
      /// the specified <see cref="Builder"/>.
      /// </summary>
      /// <param name="builder">
      /// A <see cref="Builder"/> object containing pre configured values.
      /// </param>
      public Loader(Builder builder) : base(builder) {
      }

      public Loader() : base(new Builder()) {
      }
      #endregion

      protected override void OnLoadComplete(RubySettings configuration) {
        base.OnLoadComplete(configuration);
        var local_builder = (Builder) builder;
        foreach (XmlAttribute attribute in element.Attributes) {
          if (attribute.Name.CompareOrdinalIgnoreCase(Strings.kRunningModeSwitch)) {
            local_builder.SetRunningMode(GetRunningMode(attribute));
          } else if (attribute.Name.CompareOrdinalIgnoreCase(Strings.kLogLevel)) {
            local_builder.SetLoggerLevel(GetLoggerLevel(attribute));
          } else if (attribute.Name.CompareOrdinalIgnoreCase(Strings.kCulture)) {
            local_builder.SetCulture(new CultureInfo(attribute.Name));
          }
        }
      }

      RunningMode GetRunningMode(XmlAttribute attribute) {
        RunningMode running_mode = RunningMode.Service;
        if (attribute.Value.CompareOrdinalIgnoreCase("interactive")) {
          running_mode = RunningMode.Interactive;
        }
        return running_mode;
      }

      LogLevel GetLoggerLevel(XmlAttribute attribute) {
        string log_level_string = attribute.Value.ToLower();
        switch (log_level_string) {
          case "all":
            return LogLevel.All;
          case "trace":
            return LogLevel.Trace;
          case "debug":
            return LogLevel.Debug;
          case "info":
            return LogLevel.Info;
          case "warn":
            return LogLevel.Warn;
          case "error":
            return LogLevel.Error;
          case "fatal":
            return LogLevel.Fatal;
          case "off":
            return LogLevel.Off;
          default:
            return LogLevel.Info;
        }
      }
    }
  }
}
