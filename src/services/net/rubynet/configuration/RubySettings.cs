using System;
using System.IO;
using System.Xml;
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
    RunningMode running_mode_;
    string services_folder_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="RubySettings"/> class.
    /// </summary>
    public RubySettings() {
      running_mode_ = RunningMode.Service;
      prompt_ = Strings.kShellPrompt;
      services_folder_ = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
        Strings.kServicesFolderName);
    }
    #endregion

    /// <inheritdoc/>
    public RunningMode RunningMode {
      get { return running_mode_; }
    }

    /// <inheritdoc/>
    public string ServicesFolder {
      get { return services_folder_; }
      protected set { services_folder_ = value; }
    }

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
  }
}
