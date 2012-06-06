using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

using Nohros.Desktop;
using Nohros.MyToolsPack.Console;

namespace Nohros.Ruby.Service.Net
{
  /// <summary>
  /// A class used to instantiate the shell commands.
  /// </summary>
  internal class InternalCommandsFactory : AbstractCommandFactory
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="InternalCommandsFactory"/>
    /// class.
    /// </summary>
    public InternalCommandsFactory() {
      dispatch_table_.Add(ShellSwitches.kStartCommand, CreateStartCommand);
      dispatch_table_.Add(ShellSwitches.kStopCommand, CreateStopCommand);
      dispatch_table_.Add(ShellSwitches.kHelpCommand, CreateHelpCommand);
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
      CommandLine command_line = CommandLine.FromString(command_line_args);

      string assembly_path = command_line.GetSwitchValue(
        ShellSwitches.kAssemblyNameSwitch);

      string class_type_name = command_line.GetSwitchValue(
        ShellSwitches.kTypeNameSwitch);

      // check if we have minimal information to start the service.
      if (assembly_path == string.Empty || class_type_name == string.Empty)
        throw new ArgumentException(Resources.log_shell_start_required_args);

      // if the path is not absolute it must be relative to the application
      // base diractory.
      if (!Path.IsPathRooted(assembly_path))
        assembly_path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
          assembly_path);

      if (!File.Exists(assembly_path))
        throw new ArgumentException(
          string.Format(Resources.log_assembly_not_found, assembly_path));

      // builds a command line to pass to the assembly. The command line
      // program argument will be set to the class_type_name. So, the
      // service could use a single instance of the service factory to create
      // all the services that it implements.
      string service_command_line = class_type_name;
      IList<string> loose_values = command_line.LooseValues;
      for (int i = 0, j = loose_values.Count; i < j; i++) {
        service_command_line += string.Concat(" ", loose_values[i]);
      }

      // attempt to load the specified assembly and get the service class type.
      // The type check will not performed here, the start command is
      // responsible to do this.
      Assembly service_assembly = Assembly.LoadFrom(assembly_path);
      Type service_class_type = service_assembly.GetType(class_type_name);
      if (service_class_type == null || service_assembly == null) {
        throw new TypeLoadException(
          string.Format(Resources.log_type_load_failed,
            service_assembly == null ? assembly_path : class_type_name));
      }
      return new StartCommand(service_class_type, service_command_line);
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

      string service_name = command_line.GetSwitchValue(ShellSwitches.kService);
      return new StopCommand(service_name);
    }

    public HelpCommand CreateHelpCommand(string command_line_args) {
      CommandLine command_line = CommandLine.FromString(command_line_args);
      HelpCommand command = new HelpCommand(
        command_line_args.Replace(ShellSwitches.kHelpCommand, "").Trim());
      return command;
    }
  }
}