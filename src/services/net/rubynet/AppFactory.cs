using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Reflection;
using Nohros.Configuration;
using Nohros.Desktop;
using Nohros.Logging;
using Nohros.MyToolsPack.Console;
using Nohros.Providers;

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

    const string kLogFileName = "rubynet.log";

    public const string kShellPrompt = "rubynet$: ";

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="AppFactory"/> object.
    /// </summary>
    public AppFactory() {
    }
    #endregion

    /// <summary>
    /// Loads the application setting by reading and parsing the configuration
    /// file.
    /// </summary>
    /// <returns>A <see cref="RubySettings"/> object contained the application
    /// settings.</returns>
    RubySettings CreateRubySettings() {
      // The rubynet process is started from another process(ruby service)
      // and the directory where the ruby configuration file is stored
      // could be different from the app base directory. So, instead to use
      // the AppDomain.BaseDirectory we need to use the assembly location
      // as the base directory for the configuration file.
      RubySettings settings = new RubySettings();
      settings.Load(
        Path.Combine(
          Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
          kConfigurationFileName), kConfigRootNodeName);
      return settings;
    }

    ServiceConsole CreateServiceConsole() {
      Process current_process = Process.GetCurrentProcess();
      string ipc_channel_name = "\\\\.\\pipe\\" + current_process.Id;
      try {
        NamedPipeServerStream ipc_channel =
          new NamedPipeServerStream(ipc_channel_name, PipeDirection.InOut, 1,
            PipeTransmissionMode.Message, PipeOptions.Asynchronous);
        return new ServiceConsole(ipc_channel);
      } catch (IOException) {
        #region : logging :
        RubyLogger.ForCurrentProcess.Error(
          string.Format(Resources.log_pipe_already_exists, ipc_channel_name));
        #endregion
      }
      throw new ApplicationException(Resources.log_console_creation_failed);
    }

    /// <summary>
    /// Creates and configures the shell application.
    /// </summary>
    /// <param name="command_line">The parsed command line.</param>
    /// <returns>A <see cref="RubyShell"/> object.</returns>
    public RubyShell CreateShellAsShell(CommandLine command_line) {
      RubySettings settings = CreateRubySettings();

      ConfigureLogger(settings);

      IMyToolsPackConsole console;
      console = with_shell
        ? GetToolsPackConsole(settings)
        : GetServiceConsole();

      RubyShell shell = new RubyShell(settings, console);

      // Tell the tools pack console to send our implementation of
      // the IMyToolsPackConsole interface when run commands.
      console.ToolsPackConsole = shell;

      return shell;
    }

    RubyShell CreateShellAsService(CommandLine command_line) {
      bool with_shell = command_line.HasSwitch(ShellSwitches.kWithShell); 
    }

    IMyToolsPackConsole GetToolsPackConsole(RubySettings settings) {
      // Create an instance of the class that handle the console commands.
      MyToolsPackConsole tools_pack_console =
        new MyToolsPackConsole(settings, new SystemConsole());

      // Load the my tools pack internal commands in the context of the ruby
      // console.
      tools_pack_console.LoadInternalCommands();

      // load the ruby internal commands
      ICommandFactory factory = new InternalCommandsFactory();
      foreach (string name in factory.CommandNames) {
        tools_pack_console.LoadCommand("Nohros.Ruby", name, factory);
      }
      return tools_pack_console;
    }

    void ConfigureLogger(RubySettings settings) {
      IProviderNode provider = settings.Providers[Strings.kLoggingProviderNode];
      RubyLogger.ForCurrentProcess = new RubyLogger(
        ProviderFactory<ILoggerFactory>
          .CreateProviderFactory(provider)
          .CreateLogger(provider.Options, settings));
    }
  }
}
