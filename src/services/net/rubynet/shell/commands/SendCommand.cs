using System;
using Nohros.Desktop;

namespace Nohros.Ruby
{
  internal class SendCommand: ShellCommand
  {
    internal const string kCommandName = "send";

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="SendCommand"/> class
    /// by using the specified command name and command line.
    /// </summary>
    /// <param name="name">The name of the command.</param>
    /// <param name="command_line">A <see cref="CommandLine"/> object
    /// representing command switches.
    /// </param>
    /// <seealso cref="CommandLine"/>
    public SendCommand() : base(kCommandName) { }
    #endregion

    public override void Run(ShellRubyProcess process) {
      throw new NotImplementedException();
    }
  }
}