using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

using Nohros.Configuration;

namespace Nohros.Ruby.Service.Net
{
  /// <summary>
  /// A class used to parse and manage the configuration settings file.
  /// </summary>
  class RubySettings: NohrosConfiguration
  {
    #region settings key names
    public const string kStopServiceTimeout = "stop-service-timeout";
    #endregion

    int stop_service_timeout_;
    RunningMode running_mode_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="RubySettings"/> class.
    /// </summary>
    public RubySettings() {
      stop_service_timeout_ = 3000;
    }
    #endregion

    /// <summary>
    /// Set the values of some elements that can not be set directly by the
    /// base class.
    /// </summary>
    /// <param name="e"></param>
    protected override void OnLoadComplete(EventArgs e) {
      base.OnLoadComplete(e);

      foreach (XmlAttribute attribute in element_.Attributes) {
        if (string.Compare(attribute.Name, "running-mode",
          StringComparison.OrdinalIgnoreCase) == 0) {

            if (string.Compare(attribute.Value, "interactive",
              StringComparison.OrdinalIgnoreCase) == 0) {
                running_mode_ = RunningMode.Interactive;
            } else {
              // The default running mode is "service".
              running_mode_ = RunningMode.Service;
            }
        }
      }
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
      internal set { running_mode_ = value; } // could be set to the app factory
    }
  }
}