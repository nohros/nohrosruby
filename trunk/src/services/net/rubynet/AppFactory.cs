using System;
using System.Net;
using System.Net.Sockets;
using Nohros.Configuration;
using Nohros.Extensions;
using Nohros.Logging;
using Nohros.MyToolsPack.Console;
using Nohros.Ruby.Data;
using Nohros.Ruby.Shell;
using ZmqContext = ZMQ.Context;

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

    readonly RubySettings settings_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="AppFactory"/> object.
    /// </summary>
    public AppFactory(RubySettings settings) {
      settings_ = settings;
    }
    #endregion

    ShellSelfHostProcess CreateShellSelfHostProcess(ZmqContext context) {
      var self_host_message_channel =
        new HostMessageChannel(context,
          new ZMQEndPoint(settings_.SelfHostEndpoint));
      var shell_ruby_process = CreateShellProcess(self_host_message_channel);
      var self_host_process =
        CreateSelfHostProcess(self_host_message_channel, context);
      return new ShellSelfHostProcess(shell_ruby_process,
        self_host_process);
    }

    ShellRubyProcess CreateShellProcess(IRubyMessageChannel ruby_message_channel) {
      MyToolsPackConsole console = CreateToolsPackConsole();
      var shell_ruby_process = new ShellRubyProcess(console, settings_,
        ruby_message_channel);

      // Tell the tools pack console to send our implementation of
      // the IMyToolsPackConsole interface when run commands.
      console.ToolsPackConsole = shell_ruby_process;

      return shell_ruby_process;
    }

    public IRubyProcess CreateProcess() {
      var context = new ZmqContext();
      IRubyMessageChannel ruby_message_channel = CreateMessageChannel(context);
      switch (settings_.RunningMode) {
        case RunningMode.Service:
          if (settings_.SelfHost) {
            return CreateServiceSelfHostProcess(context);
          }
          return CreateServiceProcess(ruby_message_channel);

        case RunningMode.Interactive:
          if (settings_.SelfHost) {
            return CreateShellSelfHostProcess(context);
          }
          return CreateShellProcess(ruby_message_channel);
      }
      throw new ArgumentException();
    }

    IRubyProcess CreateServiceSelfHostProcess(ZmqContext context) {
      var self_host_message_channel =
        new HostMessageChannel(context,
          new ZMQEndPoint(settings_.SelfHostEndpoint));

      // reuse the address to prevent 'address already in use' error, this
      // tells the socket to receive any message that arrives at the bound
      // port.
      var udp_client = new UdpClient();
      udp_client.Client.SetSocketOption(SocketOptionLevel.Socket,
        SocketOptionName.ReuseAddress, true);
      udp_client.Client.Bind(
        new IPEndPoint(IPAddress.Any, settings_.DiscovererPort));

      var services_repository = CreateServicesRepository();
      var tracker_factory = new TrackerFactory(context);
      var broadcaster = new Broadcaster(udp_client)
      {
        BroadcastPort = settings_.BroadcastPort
      };
      var trackers = new TrackerEngine(tracker_factory, services_repository,
        broadcaster);
      trackers.EnableTracker = settings_.EnableTracker;
      return new ServiceSelfHostProcess(settings_, self_host_message_channel,
        trackers);
    }

    SelfHostProcess CreateSelfHostProcess(
      HostMessageChannel host_message_channel, ZmqContext context) {
      // reuse the address to prevent 'address already in use' error, this
      // tells the socket to receive any message that arrives at the bound
      // port.
      var udp_client = new UdpClient();
      udp_client.Client.SetSocketOption(SocketOptionLevel.Socket,
        SocketOptionName.ReuseAddress, true);
      udp_client.Client.Bind(
        new IPEndPoint(IPAddress.Any, settings_.DiscovererPort));

      var services_repository = CreateServicesRepository();
      var tracker_factory = new TrackerFactory(context);
      var broadcaster = new Broadcaster(udp_client) {
        BroadcastPort = settings_.BroadcastPort
      };
      var trackers = new TrackerEngine(tracker_factory, services_repository,
        broadcaster);
      trackers.EnableTracker = settings_.EnableTracker;
      return new SelfHostProcess(settings_, host_message_channel, trackers);
    }

    IServicesRepository CreateServicesRepository() {
      IProviderNode provider = settings_.Providers
        .GetProviderNode(Strings.kServicesRepositoryNodeName);
      return
        RuntimeTypeFactory<IServicesRepositoryFactory>
          .CreateInstanceFallback(provider, settings_)
          .CreateServicesRepository(provider.Options.ToDictionary());
    }

    IRubyMessageChannel CreateMessageChannel(ZmqContext context) {
      if (settings_.IPCEndpoint != string.Empty) {
        return new RubyMessageChannel(context, settings_.IPCEndpoint);
      }
      return new NullMessageChannel();
    }

    /// <summary>
    /// Creates an instance of the <see cref="ServiceRubyProcess"/> class.
    /// </summary>
    /// <returns></returns>
    ServiceRubyProcess CreateServiceProcess(
      IRubyMessageChannel ruby_message_channel) {
      return new ServiceRubyProcess(settings_, ruby_message_channel);
    }

    MyToolsPackConsole CreateToolsPackConsole() {
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
