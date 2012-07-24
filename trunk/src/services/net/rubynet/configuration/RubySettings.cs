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
    string node_directory_;
    IAggregatorService aggregator_service_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="RubySettings"/> class.
    /// </summary>
    public RubySettings() {
      running_mode_ = RunningMode.Service;
      prompt_ = Strings.kShellPrompt;

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
    public string GetAbsolutePath(string path) {
      return Path.GetFullPath(Path.Combine(node_directory_, path));
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
