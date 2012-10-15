using System;
using System.Globalization;
using System.Xml;
using Nohros.Configuration;
using Nohros.Logging;

using S = Nohros.Strings;

namespace Nohros.Ruby
{
  internal partial class RubySettings
  {
    public class Loader : MustConfigurationLoader<RubySettings>
    {
      protected override void OnLoadComplete(RubySettings configuration) {
        base.OnLoadComplete(configuration);

        foreach (XmlAttribute attribute in element.Attributes) {
          if (S.AreEquals(attribute.Name, Strings.kRunningMode)) {
            configuration.running_mode_ = GetRunningMode(attribute);
          } else if (S.AreEquals(attribute.Name, Strings.kLogLevel)) {
            configuration.service_logger_level_ = GetLogLevel(attribute);
          } else if (S.AreEquals(attribute.Name, Strings.kCulture)) {
            configuration.culture_ = new CultureInfo(attribute.Name);
          }
        }
      }

      RunningMode GetRunningMode(XmlAttribute attribute) {
        RunningMode running_mode = RunningMode.Service;
        if (S.AreEquals(attribute.Value, "interactive")) {
          running_mode = RunningMode.Interactive;
        }
        return running_mode;
      }

      LogLevel GetLogLevel(XmlAttribute attribute) {
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