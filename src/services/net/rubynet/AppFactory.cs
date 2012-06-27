using System;
using System.IO;
using System.Reflection;
using Nohros.Configuration;
using Nohros.Desktop;
using Nohros.Logging;
using Nohros.MyToolsPack.Console;
using Nohros.Providers;
using Nohros.Ruby.Shell;
using ZMQ;

namespace Nohros.Ruby
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

    readonly CommandLine switches_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="AppFactory"/> object.
    /// </summary>
    public AppFactory(CommandLine switches) {
      switches_ = switches;
    }
    #endregion

    /// <summary>
    /// Loads the application setting by reading and parsing the configuration
    /// file.
    /// </summary>
    /// <returns>A <see cref="RubySettings"/> object contained the application
    /// settings.</returns>
    public RubySettings CreateRubySettings() {
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

      ConfigureLogger(settings);
      return settings;
    }

    /// <summary>
    /// Creates an instacne of the <see cref="ShellRubyProcess"/> class.
    /// </summary>
    /// <returns>
    /// A <see cref="ShellRubyProcess"/> object.
    /// </returns>
    public ShellRubyProcess CreateShellRubyProcess(RubySettings settings) {
      MyToolsPackConsole console = GetToolsPackConsole(settings);
      IRubyMessageChannel ruby_message_channel = GetIPCChannel(settings);
      ShellRubyProcess process = new ShellRubyProcess(console,
        ruby_message_channel);

      // Tell the tools pack console to send our implementation of
      // the IMyToolsPackConsole interface when run commands.
      console.ToolsPackConsole = process;

      return process;
    }

    /// <summary>
    /// Creates an instance of the <see cref="ServiceRubyProcess"/> class.
    /// </summary>
    /// <returns></returns>
    public ServiceRubyProcess CreateServiceRubyProcess(RubySettings settings) {
      IRubyMessageChannel channel = GetIPCChannel(settings);
      return new ServiceRubyProcess(channel);
    }

    IRubyMessageChannel GetIPCChannel(RubySettings settings) {
      if (switches_.HasSwitch(Strings.kIPCChannelAddress)) {
        string ipc_channel_address =
          switches_.GetSwitchValue(Strings.kIPCChannelAddress);
        Context context = new Context(Context.DefaultIOThreads);
        Socket sender = context.Socket(SocketType.REQ);
        sender.Connect(ipc_channel_address);

        Socket socket = context.Socket(SocketType.DEALER);
        socket.Connect(ipc_channel_address);

        return new RubyMessageChannel(socket);
      }
      return new NullMessageChannel();
    }

    MyToolsPackConsole GetToolsPackConsole(RubySettings settings) {
      // Create an instance of the class that handle the console commands.
      MyToolsPackConsole tools_pack_console =
        new MyToolsPackConsole(settings, new SystemConsole());

      // Load the my tools pack internal commands in the context of the ruby
      // console.
      tools_pack_console.LoadInternalCommands();

      // load the ruby internal commands
      ICommandFactory factory = new InternalCommandsFactory(settings);
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
