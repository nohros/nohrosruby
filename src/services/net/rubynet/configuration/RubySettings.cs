using System;
using System.Globalization;
using System.IO;
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
                                        IRubySettings, ITrackerSettings
  {
    public const int kDefaultSelfHostPort = 8520;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="RubySettings"/> class.
    /// </summary>
    public RubySettings(Builder builder) : base(builder) {
      // By default the language specific service host is stored at path:
      // "node_services_directory\hosts\language_name\"
      Culture = builder.Culture;
      LoggerLevel = builder.LogLevel;
      RunningMode = builder.RunningMode;
      SelfHost = builder.SelfHost;
      ServicesFolder = builder.ServiceFolder;
      Prompt = builder.Prompt;
      SelfHostEndpoint = builder.SelfHostEndpoint;
      NodeDirectory =
        Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + "..\\..\\");
      DiscovererPort = builder.BroadcastPort;
      IPCEndpoint = builder.IPCEndpoint;
      BroadcastPort = builder.BroadcastPort;
      EnableTracker = builder.EnableTracker;
      LoggingChannel = builder.LoggingChannel;
    }
    #endregion

    public RunningMode RunningMode { get; private set; }
    public LogLevel LoggerLevel { get; private set; }
    public string ServicesFolder { get; private set; }
    public string NodeDirectory { get; private set; }
    public CultureInfo Culture { get; private set; }
    public string IPCEndpoint { get; private set; }
    public string SelfHostEndpoint { get; private set; }
    public bool SelfHost { get; private set; }
    public int DiscovererPort { get; private set; }
    public string SenderEndpoint { get; private set; }
    public int BroadcastPort { get; private set; }
    public bool EnableTracker { get; private set; }
    public string LoggingChannel { get; private set; }
  }
}
