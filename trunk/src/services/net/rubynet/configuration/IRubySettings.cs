﻿using System;

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
  }
}