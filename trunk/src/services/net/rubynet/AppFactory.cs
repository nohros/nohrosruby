using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;
using System.Threading;

using MyToolsPack.Console;

using Nohros.Desktop;
using Nohros.Logging;

namespace Nohros.Ruby.Service.Net
{
  /// <summary>
  /// The application factory. Used to build the main application objects
  /// graph.
  /// </summary>
  internal class AppFactory
  {
    // the name of the rubynet configuration file
    public const string kConfigurationFileName = "rubynet.config";

    //the name of the root node of the configuration file.
    public const string kConfigRootNodeName = "rubynet";

    public const string kShellPrompt = "rubynet$: ";

    RubySettings settings_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="AppFactory"/> object.
    /// </summary>
    public AppFactory() { }
    #endregion

    /// <summary>
    /// Loads the application setting by reading and parsing the configuration
    /// file.
    /// </summary>
    /// <returns>A <see cref="RubySettings"/> object contained the application
    /// settings.</returns>
    public RubySettings RubySettings {
      get {
        if (settings_ == null) {
          Interlocked.CompareExchange<RubySettings>(ref settings_,
            new RubySettings(), null);

          settings_.Load(GetSettingsFileInfo(), kConfigRootNodeName);
        }
        return settings_;
      }
    }

    FileInfo GetSettingsFileInfo() {
      // the rubynet process is started from another process(ruby service)
      // and the directory where the ruby configuration file is stored
      // could be different from the app base directory. So, instead to use
      // the AppDomain.BaseDirectory we need to use the assembly location
      // as the base directory for the configuration file.
      string config_file_path = Path.Combine(
        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
          kConfigurationFileName);

      return new FileInfo(config_file_path);
    }

    /// <summary>
    /// Creates and configures the shell application.
    /// </summary>
    /// <param name="command_line">The parsed command line.</param>
    /// <returns>A <see cref="RubyShell"/> object.</returns>
    public RubyShell CreateShell(CommandLine command_line) {
      RubySettings settings = RubySettings;

      bool with_shell = command_line.HasSwitch(ShellSwitches.kWithShell);

      IRubyLogger logger;
      IConsole console;

      // initialize the classes that depends on the running mode.
      if (with_shell) {
        logger = RubyLogger.ForCurrentProcess =
          new RubyLogger(ColoredConsoleFileLogger.ForCurrentProcess);

        // configures the system console colors
        System.Console.ForegroundColor = ConsoleColor.DarkGreen;
        System.Console.Clear();

        console = new SystemConsole();
      } else {
        logger = RubyLogger.ForCurrentProcess =
          new RubyLogger(FileLogger.ForCurrentProcess);

        console = new ServiceConsole();
      }

      RubyLogger.ForCurrentProcess = logger;

      // load the my tools pack configuration object
      ConsoleSettings console_settings = new ConsoleSettings();
      console_settings.Load(GetSettingsFileInfo(), kConfigRootNodeName);

      // create an instance of the class that handle the console commands.
      MyToolsPackConsole tools_pack_console =
        new MyToolsPackConsole(console_settings, console);

      // load the my tools pack internal commands in the context of
      // the ruby console.
      tools_pack_console.LoadInternalCommands();

      // load the ruby internal commands
      ICommandFactory factory = new InternalCommandsFactory();
      foreach (string name in factory.CommandNames) {
        tools_pack_console.LoadCommand("Nohros.Ruby", name, factory);
      }

      RubyShell shell = new RubyShell(settings, tools_pack_console);

      // tell the tools pack console to send out implementation of
      // the IMyToolsPackConsole interface when run commands.
      tools_pack_console.ToolsPackConsole = shell;

      return shell;
    }
  }
}