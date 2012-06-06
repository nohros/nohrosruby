using System;
using System.Xml;
using Nohros.Configuration;
using Nohros.MyToolsPack.Console;

namespace Nohros.Ruby.Service.Net
{
  /// <summary>
  /// A class used to parse and manage the configuration settings file.
  /// </summary>
  internal class RubySettings : MustConfiguration, IConsoleSettings
  {
    public const string kStopServiceTimeout = "stop-service-timeout";

    string prompt_;
    RunningMode running_mode_;
    long stop_service_timeout_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="RubySettings"/> class.
    /// </summary>
    public RubySettings() {
      running_mode_ = RunningMode.Service;
      stop_service_timeout_ = 3000;
      prompt_ = "rubynet$:";
    }
    #endregion

    #region IConsoleSettings Members
    string IConsoleSettings.Prompt {
      get { return prompt_; }
    }
    #endregion

    /// <summary>
    /// Set the values of some elements that can not be set directly by the
    /// base class.
    /// </summary>
    protected override void OnLoadComplete() {
      base.OnLoadComplete();
      running_mode_ = GetRunningMode();
    }

    RunningMode GetRunningMode() {
      RunningMode running_mode = RunningMode.Service;
      foreach (XmlAttribute attribute in element.Attributes) {
        if (string.Compare(attribute.Name, "running-mode",
          StringComparison.OrdinalIgnoreCase) == 0) {
          if (string.Compare(attribute.Value, "interactive",
            StringComparison.OrdinalIgnoreCase) == 0) {
            running_mode = RunningMode.Interactive;
          }
        }
      }
      return running_mode;
    }

    /// <summary>
    /// Gets the mode on which the application is running.
    /// </summary>
    /// <remarks>
    /// If a running mode is not specified in the configuration file this
    /// property will be set to the default value
    /// <see cref="RunningMode.Service"/>.
    /// </remarks>
    public RunningMode RunningMode {
      get { return running_mode_; }
    }

    /// <summary>
    /// Gets the maximum time duration that the service host waits a service to
    /// finish its execution.
    /// </summary>
    public long StopServerTimeout {
      get {
        return stop_service_timeout_;
      }
      protected set { stop_service_timeout_ = value; }
    }
  }
}
