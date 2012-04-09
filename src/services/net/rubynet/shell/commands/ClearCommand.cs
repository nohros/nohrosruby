using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nohros.Ruby.Service.Net
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
    /// <param name="shell"></param>
    public override void Run(RubyShell shell) {
      Console.Clear();
    }
  }
}