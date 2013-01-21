using System;
using System.IO;
using System.Reflection;
using Nohros.Configuration;
using Nohros.Extensions;
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
    const string kLogFileName = "rubynet.log";
    public const string kShellPrompt = "rubynet$: ";

    readonly IRubySettings settings_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="AppFactory"/> object.
    /// </summary>
    public AppFactory(IRubySettings settings) {
      settings_ = settings;
    }
    #endregion

    /// <summary>
    /// Creates an instacne of the <see cref="ShellRubyProcess"/> class.
    /// </summary>
    /// <returns>
    /// A <see cref="ShellRubyProcess"/> object.
    /// </returns>
    public ShellRubyProcess CreateShellRubyProcess() {
      MyToolsPackConsole console = GetToolsPackConsole();
      IRubyMessageChannel ruby_message_channel = GetIPCChannel();
      var process = new ShellRubyProcess(console, settings_,
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
    public ServiceRubyProcess CreateServiceRubyProcess() {
      IRubyMessageChannel channel = GetIPCChannel();
      return new ServiceRubyProcess(settings_, channel);
    }

    IRubyMessageChannel GetIPCChannel() {
      if (settings_.SelfHost) {
        return new SelfMessageChannel(
          new Context(Context.DefaultIOThreads), settings_.SelfHostAddress);
      }
      if (settings_.IPCChannelAddress != string.Empty) {
        return new RubyMessageChannel(
          new Context(Context.DefaultIOThreads), settings_.IPCChannelAddress);
      }
      return new NullMessageChannel();
    }

    MyToolsPackConsole GetToolsPackConsole() {
      // Create an instance of the class that handle the console commands.
      var tools_pack_console =
        new MyToolsPackConsole(settings_, new SystemConsole());

      // Load the my tools pack internal commands in the context of the ruby
      // console.
      tools_pack_console.LoadInternalCommands();

      // load the ruby internal commands
      ICommandFactory factory = new InternalCommandsFactory(settings_);
      foreach (string name in factory.CommandNames) {
        tools_pack_console.LoadCommand("Nohros.Ruby", name, factory);
      }
      return tools_pack_console;
    }

    public void ConfigureLogger() {
      IProviderNode provider;
      if (settings_.Providers.GetProviderNode(Strings.kLogProviderNode,
        out provider)) {
        // try/catch: logging related operations should not causes application
        // issues.
        try {
          RubyLogger.ForCurrentProcess.Logger =
            RuntimeTypeFactory<ILoggerFactory>
              .CreateInstanceFallback(provider, settings_)
              .CreateLogger(provider.Options.ToDictionary());
        } catch (System.Exception e) {
          // fails silently.
        }
      }
    }
  }
}
