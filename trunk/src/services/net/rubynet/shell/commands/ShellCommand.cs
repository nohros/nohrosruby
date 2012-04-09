using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MyToolsPack.Console;

using Nohros.Desktop;

namespace Nohros.Ruby.Service.Net
{
  /// <summary>
  /// A generic implementation of the <see cref="IShellCommand"/> interface.
  /// </summary>
  internal abstract class ShellCommand: IShellCommand, ICommand
  {
    string name_;
    CommandLine command_line_switches_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="ShellCommand"/> by using
    /// the specified command name.
    /// </summary>
    /// <param name="name">The name of the command</param>
    public ShellCommand(string name) {
      name_ = name;
    }

    public ShellCommand(string name, CommandLine switches) {
      command_line_switches_ = switches;
    }
    #endregion

    /// <inherithdoc/>
    void ICommand.Run(IMyToolsPackConsole console) {
      Run(console as RubyShell);
    }

    /// <summary>
    /// Runs the command.
    /// </summary>
    public abstract void Run(RubyShell shell);

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