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

      switch (settings.RunningMode) {
        case RunningMode.Service:
          factory.CreateServiceRubyProcess().Run(legacy_command_line_string);
          break;

        case RunningMode.Interactive:
          factory.CreateServiceRubyProcess().Run(legacy_command_line_string);
          break;

        default:
          throw new NotReachedException();
      }
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
        .GetSwitchValue(Strings.kConfigFileRootName,
          Strings.kConfigFileRootSwitch);

      var builder = new RubySettings.Builder();
      if (switches.HasSwitch(Strings.kIPCChannelAddressSwitch)) {
        builder
          .SetIPCChannelAddress(
            switches.GetSwitchValue(Strings.kIPCChannelAddressSwitch));
      }
      return new RubySettings.Loader(builder)
        .Load(Path.AbsoluteForCallingAssembly(config_file_name),
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
