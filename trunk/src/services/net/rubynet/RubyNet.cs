using System;
using System.Collections.Generic;

using Nohros.Desktop;

namespace Nohros.Ruby
{
  static class RubyNet
  {
    const string kVersion = @"
RubyNet Version 0.3.0
";

    internal const string kHeader = kVersion + @"
Copyright (c) 2010 Nohros Systems Inc. All Rights Reserved.

Use of this software code is governed by a MIT license.

";

    internal const string kUsageCommon = @"
  -assembly      specifes the assembly to load and run. This value must be
                 an absolute path or a path relative to the base directory.

  -type          specifies the fully qualified type name of a class
                 that implements the IRubyService interface.

   ARGS          A list of arguments to pass to the loaded assembly. The
                 list of arguments must be preceded by a '-- ' argument
                 (without quotes).
";

    const string kUsage = kHeader + @"
Runs a .NET assembly like a console application.

Usage: nohros.rubynet -assembly=ASSEMBLYNAME -type=TYPENAME [-help] -- ARGS
"
        + kUsageCommon +
@"
  -with-shell    specifies that the a command line language interpreter
                 must be started. With a shell users can stop and start
                 services directly from the command line and without
                 the intervention of a ruby net agent. It can to send
                 commands to a running service. Check out the full
                 documentation to know how to do it.

  -help          Displays this help and exit.
  
  -version       Displays the version and exit.

   Examples:
     nohros.rubynet -assembly=my.assembly.dll -type=my.type, my.namespace
                    -- -debug -path=c:\\path\\

     The my.assembly will be loaded into the rubynet domain and a new
     instance of the my.type will be created. If the type instantiation is
     successful then the Run method will be called and the program control
     will be transfered to the loaded assembly.

     Do not attempt to start a nohros.rubynet program using the nohros.rubynet.
";

    const string kAssemblyNameSwitch = "assembly";
    const string kTypeNameSwitch = "type";
    const string kHelpSwitch = "help";
    const string kDebugSwitch = "debug";
    const string kWithShellSwitch = "with-shell";

    internal const string kPipeSwitch = "pipe";

    static void Main(string[] args) {
      CommandLine command_line = CommandLine.ForCurrentProcess;

      // Check if the debug is enabled first, so the caller has the chance
      // to debug everything.
      if (command_line.HasSwitch(kDebugSwitch))
        System.Diagnostics.Debugger.Launch();

      // Show the usage tips if desired.
      if (command_line.HasSwitch(kHelpSwitch))
        Console.WriteLine(kHelpSwitch);

      // We cannot control the behavior of the service. For, logger purposes,
      // we will monitor all the unhandled application exception. Note that
      // this does not prevents the application from shutting down, its is used
      // only for logging purposes.
      AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

      // Legacy version of this app was used to start a single service without
      // a process. For compatbility with this legacy app we need to send the
      // command line to the run method simulating the start command.
      string legacy_command_line_string = GetLegacyCommandLine(command_line);

      // Checks if the process should runs as a service or process.
      bool run_as_shell = command_line.HasSwitch(ShellSwitches.kWithShell);

      AppFactory factory = new AppFactory(command_line);
      IRubyProcess process = run_as_shell
        ? factory.CreateShellRubyProcess() as IRubyProcess
        : factory.CreateServiceRubyProcess() as IRubyProcess;
      process.Run(legacy_command_line_string);
    }

    /// <summary>
    /// Transform the specified command line into a command line that could be
    /// be translated to a start command.
    /// </summary>
    /// <param name="current_process_command_line">The current application
    /// command line.</param>
    /// <returns>A string that could be translated to a start command by the
    /// shell command line parser.</returns>
    static string GetLegacyCommandLine(
      CommandLine current_process_command_line) {

      // check if the required start parameters was supplied.
      if (current_process_command_line.HasSwitch(
        ShellSwitches.kAssemblyNameSwitch) &&
        current_process_command_line.HasSwitch(
        ShellSwitches.kTypeNameSwitch)) {

        CommandLine start_command_line =
          new CommandLine(ShellSwitches.kStartCommand);

        start_command_line.CopySwitchesFrom(current_process_command_line);

        // append the switch parsing prefix to the command line ensuring
        // taht only the loose parameters is sent to the service.
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