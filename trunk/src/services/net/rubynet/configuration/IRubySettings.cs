using System;
using System.Globalization;
using Nohros.Logging;
using Nohros.Configuration;

namespace Nohros.Ruby
{
  /// <summary>
  /// <see cref="IRubySettings"/> defines the application settings.
  /// </summary>
  internal interface IRubySettings : IMustConfiguration
  {
    /// <summary>
    /// Gets the mode on which the application is running.
    /// </summary>
    /// <remarks>
    /// If a running mode is not specified in the configuration file will
    /// assume that we are running as a service.
    /// </remarks>
    RunningMode RunningMode { get; }

    /// <summary>
    /// Gets the level of the logger that is used by the services
    /// </summary>
    /// <seealso cref="IRubyServiceHost.Logger"/>
    LogLevel ServiceLoggerLevel { get; }

    /// <summary>
    /// Gets the name of the folder that is used to store the services
    /// binaries.
    /// </summary>
    string ServicesFolder { get; }

    /// <summary>
    /// Gets the full path to the services' node directory.
    /// </summary>
    /// <remarks>
    /// This is the working directory of the service node executable.
    /// </remarks>
    string NodeDirectory { get; }

    /// <summary>
    /// Gets the configured culture.
    /// </summary>
    CultureInfo Culture { get; }

    /// <summary>
    /// Gets an instance of the <see cref="IAggregatorService"/> class that can
    /// be used to send message to the log aggregator service.
    /// </summary>
    IAggregatorService AggregatorService { get; set; }
  }
}
