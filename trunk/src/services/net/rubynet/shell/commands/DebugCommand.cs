using System;
using System.Diagnostics;
using Nohros.Ruby.Shell;

namespace Nohros.Ruby
{
  /// <summary>
  /// Launches and attaches a debugger to the process.
  /// </summary>
  internal class DebugCommand : ShellCommand
  {
    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="DebugCommand"/>.
    /// </summary>
    public DebugCommand() : base(ShellStrings.kDebugCommand) {
    }
    #endregion

    /// <summary>
    /// Launches and attaches a debugger to the process.
    /// </summary>
    /// <param name="process">
    /// The <see cref="ShellRubyProcess"/> that is running the command.
    /// </param>
    public override void Run(ShellRubyProcess process) {
      Debugger.Launch();
    }
  }
}
