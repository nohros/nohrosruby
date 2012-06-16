using System;
using System.IO;
using System.Reflection;
using Nohros.Configuration;
using Nohros.Desktop;
using Nohros.Logging;
using Nohros.MyToolsPack.Console;
using Nohros.Providers;
using Nohros.Ruby.Service.Net;
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

    /// <summary>
    /// Creates an instacne of the <see cref="ShellRubyProcess"/> class.
    /// </summary>
    /// <returns>
    /// A <see cref="ShellRubyProcess"/> object.
    /// </returns>
    public ShellRubyProcess CreateShellRubyProcess() {
      RubySettings settings = CreateRubySettings();
      ConfigureLogger(settings);

      MyToolsPackConsole console = GetToolsPackConsole(settings);
      IPCChannel ipc_channel = GetIPCChannel(settings);
      ShellRubyProcess process = new ShellRubyProcess(console, ipc_channel);

      // Tell the tools pack console to send our implementation of
      // the IMyToolsPackConsole interface when run commands.
      console.ToolsPackConsole = process;

      return process;
    }

    /// <summary>
    /// Creates an instance of the <see cref="ServiceRubyProcess"/> class.
    /// </summary>
    /// <returns></returns>
    public ServiceRubyProcess CreateServiceRubyProcess() {
      RubySettings settings = CreateRubySettings();
      ConfigureLogger(settings);
      IPCChannel channel = GetIPCChannel(settings);
      return new ServiceRubyProcess(channel);
    }

    IPCChannel GetIPCChannel(RubySettings settings) {
      IRubyMessageSender sender;
      IRubyMessageReceiver receiver;
      Context context = null;
      if (switches_.HasSwitch(Strings.kMessageListenerEndpoint)) {
        context = new Context(Context.DefaultIOThreads);
        Socket socket = context.Socket(SocketType.PUB);
        socket.Connect(switches_.GetSwitchValue(Strings.kMessageListenerEndpoint));
        sender = new RubyMessageSender(socket);
      } else {
        sender = new LoggerMessageSender();
      }

      if (switches_.HasSwitch(Strings.kMessageSenderEndpoint)) {
        if (context == null) {
          context = new Context(Context.DefaultIOThreads);
        }
        Socket socket = context.Socket(SocketType.SUB);
        socket.Connect(switches_.GetSwitchValue(Strings.kMessageSenderEndpoint));
        receiver = new RubyMessageReceiver(socket);
      } else {
        receiver = new BlockedMessageReceiver();
      }
      return new IPCChannel(sender, receiver);
    }

    MyToolsPackConsole GetToolsPackConsole(RubySettings settings) {
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
