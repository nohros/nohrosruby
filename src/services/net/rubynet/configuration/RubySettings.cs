using System;
using System.Diagnostics;
using System.Globalization;
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
    CultureInfo culture_;
    string node_directory_;
    RunningMode running_mode_;
    LogLevel service_logger_level_;
    string services_folder_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="RubySettings"/> class.
    /// </summary>
    public RubySettings(Builder builder) : base(builder) {
      running_mode_ = RunningMode.Service;
      prompt_ = Strings.kShellPrompt;
      services_folder_ = "services";
      service_logger_level_ = LogLevel.Info;
      culture_ = CultureInfo.InvariantCulture;

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
    public CultureInfo Culture {
      get { return culture_; }
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
  }
}
