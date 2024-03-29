﻿using System;
using System.Globalization;
using Nohros.Logging;
using Nohros.Configuration;
using Nohros.MyToolsPack.Console;

namespace Nohros.Ruby
{
  /// <summary>
  /// <see cref="IRubySettings"/> defines the application settings.
  /// </summary>
  public interface IRubySettings : IConfiguration, IConsoleSettings
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
    LogLevel LoggerLevel { get; }

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
    /// Gets the address of the endpoint that can be used to communicate with
    /// the service node.
    /// </summary>
    string IPCEndpoint { get; }

    /// <summary>
    /// Gets a address to use while we are selh hosting.
    /// </summary>
    string SelfHostEndpoint { get; }

    /// <summary>
    /// Gets a value indicating if the self host mode is enabled.
    /// </summary>
    /// <value><c>true</c> if the sel-host mode is enabled.</value>
    bool SelfHost { get; }

    /// <summary>
    /// Gets a value indicating if the tracker service should be enabled or not.
    /// </summary>
    /// <remarks>
    /// The tracker service acts as a bootstrap node.
    /// </remarks>
    bool EnableTracker { get; }

    /// <summary>
    /// Gets the address of teh logging server.
    /// </summary>
    /// <remarks>
    /// The logging server is usually discovered through the ruby auto discover
    /// mechanism. This setting should be used on larger networks when you have
    /// a know central loggin server and/or when the auto discoverer feature
    /// is not enabled.
    /// </remarks>
    string LoggingChannel { get; }
  }
}
