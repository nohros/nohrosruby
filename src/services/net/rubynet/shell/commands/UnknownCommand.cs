using System;

namespace Nohros.Ruby
{
  internal class UnknownCommand: ShellCommand
  {
    #region .ctor
    /// <summary>
    /// Initializes a nes instance of the <see cref="UnknownCommand"/>.
    /// </summary>
    public UnknownCommand() : base("unknown") { }
    #endregion

    /// <summary>
    /// Executes the command.
    /// </summary>
    /// <remarks>
    /// This class is used like a place holder, so this command does nothing.
    /// </remarks>
    public override void Run(ShellRubyProcess process) { }

    /// <summary>
    /// Gets the name of the command.
    /// </summary>
    /// <remarks>
    /// The name of the command that this class represents is "unknown".
    /// </remarks>
    public string Name {
      get { return "unknown"; }
    }
  }
}
