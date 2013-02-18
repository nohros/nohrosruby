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
  internal partial class RubySettings : Configuration.Configuration,
                                        IConfiguration, IConsoleSettings,
                                        IRubySettings
  {
    public const int kDefaultSelfHostPort = 8520;

    readonly CultureInfo culture_;
    readonly string ipc_channel_address_;
    readonly LogLevel logger_level_;
    readonly string node_directory_;
    readonly string prompt_;
    readonly RunningMode running_mode_;
    readonly bool self_host_;
    readonly string self_host_address_;
    readonly string services_folder_;
    readonly string tracker_address_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="RubySettings"/> class.
    /// </summary>
    public RubySettings(Builder builder) : base(builder) {
      running_mode_ = builder.RunningMode;
      prompt_ = builder.Prompt;
      services_folder_ = builder.ServiceFolder;
      logger_level_ = builder.LoggerLevel;
      culture_ = builder.Culture;
      ipc_channel_address_ = builder.IPCChannelAddress;
      self_host_address_ = builder.SelfHostAddress;
      self_host_ = builder.SelfHost;
      tracker_address_ = builder.TrackerAddress;

      // By default the language specific service host is stored at path:
      // "node_services_directory\hosts\language_name\"
      node_directory_ =
        Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + "..\\..\\");
    }
    #endregion

    public string TrackerAddress {
      get { return tracker_address_; }
    }

    public string SelfHostAddress {
      get { return self_host_address_; }
    }

    public bool SelfHost {
      get { return self_host_; }
    }

    /// <inheritdoc/>
    public RunningMode RunningMode {
      get { return running_mode_; }
    }

    /// <inheritdoc/>
    public string IPCChannelAddress {
      get { return ipc_channel_address_; }
    }

    /// <inheritdoc/>
    public CultureInfo Culture {
      get { return culture_; }
    }

    /// <inheritdoc/>
    public string ServicesFolder {
      get { return services_folder_; }
    }

    /// <inheritdoc/>
    public string NodeDirectory {
      get { return node_directory_; }
    }

    /// <inheritdoc/>
    public LogLevel LoggerLevel {
      get { return logger_level_; }
    }
  }
}
