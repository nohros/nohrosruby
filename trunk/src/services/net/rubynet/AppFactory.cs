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
      IRubyMessageChannel channel = GetMessageChannel();
      var process = new ShellRubyProcess(console, settings_, channel);

      // Tell the tools pack console to send our implementation of
      // the IMyToolsPackConsole interface when run commands.
      console.ToolsPackConsole = process;

      return process;
    }

    IRubyMessageChannel GetMessageChannel() {
      if (settings_.IPCEndpoint != string.Empty) {
        var context = new Context();
        return new RubyMessageChannel(context, settings_.IPCEndpoint);
      }
      return new NullMessageChannel();
    }

    /// <summary>
    /// Creates an instance of the <see cref="ServiceRubyProcess"/> class.
    /// </summary>
    /// <returns></returns>
    public ServiceRubyProcess CreateServiceRubyProcess() {
      var channel = GetMessageChannel();
      return new ServiceRubyProcess(settings_, channel);
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
