using System;
using System.Collections.Generic;
using Nohros.IO;
using Nohros.Ruby.Shell;

namespace Nohros.Ruby
{
  internal static class RubyNet
  {
    static void Main(string[] args) {
      CommandLine switches = CommandLine.ForCurrentProcess;

      // Check if the debug is enabled first, so the caller has the chance
      // to debug everything.
      if (switches.HasSwitch(Strings.kWaitForDebugger)) {
        System.Diagnostics.Debugger.Launch();
      }

      // Show the usage tips if desired.
      if (switches.HasSwitch(Strings.kHelp))
        Console.WriteLine(Strings.kHelp);

      // We cannot control the behavior of the service. For, logger purposes,
      // we will monitor all the unhandled application exception. Note that
      // this does not prevents the application from shutting down, its is used
      // only for logging purposes.
      AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

      // Legacy version of this app was used to start a single service without
      // a process. For compatbility with this legacy app we need to send the
      // command line to the run method simulating the start command.
      string legacy_command_line_string = GetLegacyCommandLine(switches);

      RubySettings settings = GetRubySettings(switches);
      var factory = new AppFactory(settings);
      factory.ConfigureLogger();

      IRubyProcess process = factory.CreateProcess();
      process.Run(legacy_command_line_string);
    }

    /// <summary>
    /// Loads the application setting by reading and parsing the configuration
    /// file.
    /// </summary>
    /// <returns>
    /// A <see cref="RubySettings"/> object contained the application settings.
    /// </returns>
    static RubySettings GetRubySettings(CommandLine switches) {
      // The rubynet process is started from another process(ruby service)
      // and the directory where the ruby configuration file is stored
      // could be different from the app base directory. So, instead to use
      // the AppDomain.BaseDirectory we need to use the assembly location
      // as the base directory for the configuration file.
      string config_file_name = switches
        .GetSwitchValue(Strings.kConfigFileNameSwitch, Strings.kConfigFileName);
      string config_file_root_node = switches
        .GetSwitchValue(Strings.kConfigFileRootSwitch,
          Strings.kConfigFileRootName);
      string running_mode = switches
        .GetSwitchValue(Strings.kRunningModeSwitch);
      string services_folder = switches
        .GetSwitchValue(Strings.kServicesFolderSwitch);
      string ipc_channel_endpoint = switches
        .GetSwitchValue(Strings.kIPCChannelAddressSwitch);
      string logging_channel = switches
        .GetSwitchValue(Strings.kLoggingChannel);
      int broadcast_port = switches.GetSwitchValueAsInt(
        Strings.kBroadcastPortSwitch, 0);
      bool enable_tracker = switches.HasSwitch(Strings.kEnableTrackerSwitch);

      var builder = new RubySettings.Builder();
      var loader = new RubySettings.Loader(builder);

      // override the values set by class loader.
      loader.ParseComplete += sender => {
        bool self_host = switches.HasSwitch(Strings.kSelfHostSwitch) ||
          builder.SelfHost;

        if (self_host) {
          builder.SetSelfHost(true);
          string self_host_endpoint = switches
            .GetSwitchValue(Strings.kSelfHostSwitch);
          if (self_host_endpoint != string.Empty) {
            builder.SetSelfHostEndpoint("tcp://*:" + self_host_endpoint);
          }

          // The default running mode for self hosting is interactive.
          if (running_mode == string.Empty) {
            builder.SetRunningMode(RunningMode.Interactive);
          }

          builder.SetIPCEndpoint(Strings.kSelfHostIPCEndpoint);
        }

        if (services_folder != string.Empty) {
          builder.SetServiceFolder(services_folder);
        }

        if (broadcast_port != 0) {
          builder.SetDiscovererPort(broadcast_port);
        }

        if (enable_tracker) {
          builder.SetEnableTracker(true);
        }

        if (ipc_channel_endpoint != string.Empty) {
          builder.SetIPCEndpoint(ipc_channel_endpoint);
        }

        if (logging_channel != string.Empty) {
          builder.SetLoggingChannel(logging_channel);
        }

        if (running_mode != string.Empty) {
          loader.RunningMode = running_mode;
        }
      };

      return loader.Load(Path.AbsoluteForCallingAssembly(config_file_name),
        config_file_root_node);
    }

    /// <summary>
    /// Transform the specified command line into a command line that could be
    /// be translated to a start command.
    /// </summary>
    /// <param name="current_process_command_line">The current application
    /// command line.</param>
    /// <returns>A string that could be translated to a start command by the
    /// shell command line parser.</returns>
    static string GetLegacyCommandLine(CommandLine current_process_command_line) {
      // Check if the required start parameters was supplied.
      if (current_process_command_line.HasSwitch(Strings.kServiceAssembly) &&
        current_process_command_line.HasSwitch(Strings.kServiceType)) {
        CommandLine start_command_line =
          new CommandLine(ShellStrings.kStartCommand);
        start_command_line.CopySwitchesFrom(current_process_command_line);

        // Append the switch parsing prefix to the command line ensuring that
        // only the loose parameters is sent to the service.
        start_command_line.AppendSwitchParsingStopPrefix();
        IList<string> loose_values = current_process_command_line.LooseValues;
        foreach (string loose_value in loose_values) {
          start_command_line.AppendLooseValue(loose_value);
        }
        return start_command_line.CommandLineString;
      }
      return string.Empty;
    }

    /// <summary>
    /// Logs all the application unhandled exceptions.
    /// </summary>
    /// <param name="sender">The source of the unhandled exception event.</param>
    /// <param name="e">An <see cref="UnhandledExceptionEventArgs"/> that
    /// contains the event data.</param>
    /// <remarks>The behavior of the service hosted by us, is not over our
    /// control. There is no way to prevents the application from shutting
    /// down, but we can log it. We do it here.</remarks>
    static void OnUnhandledException(object sender,
      UnhandledExceptionEventArgs e) {
      // The ExceptionObject property og the UnhandledExceptionEventArgs class
      // is not an Excepition because it is posible to throw object in .NET
      // that do not derive from System.Exception. This is possible in some
      // CLR based languages but not C#. We can safe cast it to
      // System.Exception.
      string message = "";
      Exception exception = e.ExceptionObject as Exception;
      while (exception != null) {
        // Is unusual to have a great number of inner excpetions and this
        // piece of code does not impact the application performance, so
        // using a string concatenation is OK.
        message += exception.Message;
        exception = exception.InnerException;
      }
      RubyLogger.ForCurrentProcess.Fatal(message);
    }
  }
}
