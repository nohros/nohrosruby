using System;
using System.Collections.Specialized;

namespace Nohros.Ruby.Shell
{
  internal class HelpCommand: ShellCommand
  {
    const string kCommandsList = Strings.kHeader + @"
The following commands are recognized by rubynet:

    start args
        starts a in-process service. The service will be runned into a
        dedicated thread. To show the list of arguments try help start.

    stop service-name
        stops a service whose name is service-name.

    send service-name message
        sends a message to the service.

    help command
        displays a help text of the given command.
";

    const string kStartCommandHelp = Strings.kHeader + @"
Starts a in-process service. The service will run into a dedicated thread. The
list of arguments are the following:

" + Strings.kUsageCommon;

    const string kClearCommandHelp = Strings.kHeader + @"
Clear the console windows of commands and any output generated by them. It does
not clear the user's history of commands, however.
";
    const string kExitCommandHelp = Strings.kHeader + @"
Exit from the ruby command line process.
";
    const string kSendCommandHelp = Strings.kHeader + @"
Sends a message to a running service.
";
    const string kStopCommandHelp = Strings.kHeader + @"
Stops a service.
";

    readonly string command_name_;
    readonly ListDictionary help_commands_table_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="HelpCommand"/>.
    /// </summary>
    /// <remarks>
    /// <see cref="HelpCommand"/> obejcts created by using this constructor
    /// displays the list of known commands with a brief description about
    /// them. To see the help for a particular command use one of the
    /// constructors overloads.
    /// </remarks>
    public HelpCommand() : this(string.Empty) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="HelpCommand"/> by using
    /// the specified command name.
    /// </summary>
    /// <param name="command_name">
    /// The name of the command whose help information should be displayed.
    /// </param>
    /// <remarks>
    /// If <paramref name="command_name"/> is an unregognized command this
    /// command display the list of known commands with a brief description
    /// about them.
    /// </remarks>
    public HelpCommand(string command_name)
      : base(ShellStrings.kHelpCommand) {
        command_name_ = command_name;

        help_commands_table_ = new ListDictionary(
          StringComparer.OrdinalIgnoreCase);

        help_commands_table_[ShellStrings.kStartCommand] = kStartCommandHelp;
        help_commands_table_[ShellStrings.kStopCommand] = kStopCommandHelp;
        help_commands_table_[ShellStrings.kSendCommand] = kSendCommandHelp;
        help_commands_table_[ShellStrings.kClearCommand] = kClearCommandHelp;
        help_commands_table_[ShellStrings.kExitCommand] = kExitCommandHelp;
    }
    #endregion

    /// <inherit />
    public override void Run(ShellRubyProcess process) {
      string help_text = help_commands_table_[command_name_] as string;
      if (help_text == null) {
        help_text = kCommandsList;
      }
      process.WriteLine(help_text);
    }
  }
}
