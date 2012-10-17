using System;
using Nohros.MyToolsPack.Console;

namespace Nohros.Ruby.Shell
{
  /// <summary>
  /// A generic implementation of the <see cref="IShellCommand"/> interface.
  /// </summary>
  internal abstract class ShellCommand: IShellCommand, ICommand
  {
    readonly string name_;
    CommandLine command_line_switches_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="ShellCommand"/> by using
    /// the specified command name.
    /// </summary>
    /// <param name="name">
    /// The name of the command.
    /// </param>
    protected ShellCommand(string name) {
      name_ = name;
    }

    /// <summary>
    /// Intializes a new instance of the <see cref="ShellCommand"/> by using
    /// the specified command name.
    /// </summary>
    /// <param name="name">
    /// The command's name.
    /// </param>
    /// <param name="switches">
    /// A <see cref="CommandLine"/> containing the command switches that was
    /// specified by the user.
    /// </param>
    protected ShellCommand(string name, CommandLine switches) : this(name) {
      command_line_switches_ = switches;
    }
    #endregion

    /// <inherithdoc/>
    void ICommand.Run(IMyToolsPackConsole console) {
      Run(console as ShellRubyProcess);
    }

    /// <summary>
    /// Runs the command.
    /// </summary>
    public abstract void Run(ShellRubyProcess process);

    /// <summary>
    /// Gets the name of the command.
    /// </summary>
    public virtual string Name {
      get { return name_; }
    }

    public CommandLine Switches {
      get { return command_line_switches_; }
      set { command_line_switches_ = value; }
    }
  }
}