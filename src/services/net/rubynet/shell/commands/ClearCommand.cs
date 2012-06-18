using System;

namespace Nohros.Ruby.Shell
{
  /// <summary>
  /// A command used to clear the console.
  /// </summary>
  internal class ClearCommand : ShellCommand
  {
    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="ClearCommand"/> class
    /// </summary>
    public ClearCommand():base(ShellSwitches.kClearCommand) { }
    #endregion

    /// <summary>
    /// Runs the command clearing the console.
    /// </summary>
    /// <param name="process"></param>
    public override void Run(ShellRubyProcess process) {
      Console.Clear();
    }
  }
}