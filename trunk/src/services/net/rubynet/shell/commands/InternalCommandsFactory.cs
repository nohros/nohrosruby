using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

using Nohros.Desktop;
using Nohros.MyToolsPack.Console;

namespace Nohros.Ruby.Shell
{
  /// <summary>
  /// A class used to instantiate the shell commands.
  /// </summary>
  internal class InternalCommandsFactory : AbstractCommandFactory
  {
    readonly IRubySettings settings_;

    /// <summary>
    /// Initializes a new instance of the <see cref="InternalCommandsFactory"/>
    /// class.
    /// </summary>
    public InternalCommandsFactory(IRubySettings settings) {
      settings_ = settings;
      dispatch_table_.Add(ShellStrings.kStartCommand, CreateStartCommand);
      dispatch_table_.Add(ShellStrings.kStopCommand, CreateStopCommand);
      dispatch_table_.Add(ShellStrings.kHelpCommand, CreateHelpCommand);
    }

    /// <summary>
    /// Creates a <see cref="StartCommand"/> object by parsing the command line
    /// arguments.
    /// </summary>
    /// <param name="command_line_args">
    /// A <see cref="CommandLine"/> object containing the command line
    /// arguments.
    /// </param>
    /// <returns>
    /// The newly created <see cref="StartCommand"/> object.
    /// </returns>
    public StartCommand CreateStartCommand(string command_line_args) {
      return
        new StartCommand(CommandLine.FromString(command_line_args), settings_);
    }

    /// <summary>
    /// Creates a <see cref="StopCommand"/> object by parsing the command line
    /// arguments.
    /// </summary>
    /// <param name="command_line_args">A <see cref="CommandLine"/> object
    /// containing the command line arguments.</param>
    /// <returns></returns>
    public StopCommand CreateStopCommand(string command_line_args) {
      CommandLine command_line = CommandLine.FromString(command_line_args);

      string service_name = command_line.GetSwitchValue(ShellStrings.kService);
      return new StopCommand(service_name);
    }

    public HelpCommand CreateHelpCommand(string command_line_args) {
      CommandLine command_line = CommandLine.FromString(command_line_args);
      HelpCommand command = new HelpCommand(
        command_line_args.Replace(ShellStrings.kHelpCommand, "").Trim());
      return command;
    }
  }
}