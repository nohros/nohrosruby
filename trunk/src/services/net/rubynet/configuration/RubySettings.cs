using System;
using System.IO;
using System.Xml;
using Nohros.Logging;
using Nohros.Configuration;
using Nohros.MyToolsPack.Console;

namespace Nohros.Ruby
{
  /// <summary>
  /// A class used to parse and manage the configuration settings file.
  /// </summary>
  internal partial class RubySettings : MustConfiguration, IConsoleSettings,
                                        IRubySettings
  {
    readonly string prompt_;
    IAggregatorService aggregator_service_;
    string node_directory_;
    RunningMode running_mode_;
    LogLevel service_logger_level_;
    string services_folder_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="RubySettings"/> class.
    /// </summary>
    public RubySettings() {
      running_mode_ = RunningMode.Service;
      prompt_ = Strings.kShellPrompt;
      services_folder_ = "services";
      service_logger_level_ = LogLevel.Info;

      // By default the language specific service host is stored at path:
      // "node_services_directory\hosts\language_name\"
      node_directory_ =
        Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + "..\\..\\");
    }
    #endregion

    /// <inheritdoc/>
    public RunningMode RunningMode {
      get { return running_mode_; }
    }

    /// <inheritdoc/>
    public LogLevel ServiceLoggerLevel {
      get { return service_logger_level_; }
      set { service_logger_level_ = value; }
    }

    /// <inheritdoc/>
    public string ServicesFolder {
      get { return services_folder_; }
      protected set { services_folder_ = value; }
    }

    /// <inheritdoc/>
    public string NodeDirectory {
      get { return node_directory_; }
      protected set { node_directory_ = value; }
    }

    /// <inheritdoc/>
    public IAggregatorService AggregatorService {
      get { return aggregator_service_; }
      set { aggregator_service_ = value; }
    }

    /// <summary>
    /// Set the values of some elements that can not be set directly by the
    /// base class.
    /// </summary>
    protected override void OnLoadComplete() {
      base.OnLoadComplete();
      RunningMode running_mode = RunningMode.Service;
      foreach (XmlAttribute attribute in element.Attributes) {
        if (StringsAreEquals(attribute.Name, Strings.kRunningMode)) {
          running_mode_ = GetRunningMode(attribute);
        } if (StringsAreEquals(attribute.Name, Strings.kLogLevel)) {
          service_logger_level_ = GetLogLevel(attribute);
        }
      }
    }

    RunningMode GetRunningMode(XmlAttribute attribute) {
      RunningMode running_mode = RunningMode.Service;
      if (StringsAreEquals(attribute.Value, "interactive")) {
        running_mode = RunningMode.Interactive;
      }
      return running_mode;
    }

    internal static bool StringsAreEquals(string str_a, string str_b) {
      return string.Compare(str_a, str_b, StringComparison.Ordinal) == 0;
    }

    LogLevel GetLogLevel(XmlAttribute attribute) {
      string log_level_string = attribute.Value.ToLower();
      switch(log_level_string) {
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
          return service_logger_level_;
      }
    }
  }
}
