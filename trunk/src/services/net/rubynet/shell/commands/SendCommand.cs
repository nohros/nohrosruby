using System;
using Nohros.Desktop;

namespace Nohros.Ruby.Shell
{
  internal class SendCommand: ShellCommand
  {
    internal const string kCommandName = "send";

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="SendCommand"/> class
    /// by using the specified command name and command line.
    /// </summary>
    /// <seealso cref="CommandLine"/>
    public SendCommand() : base(kCommandName) { }
    #endregion

    public override void Run(ShellRubyProcess process) {
      throw new NotImplementedException();
    }
  }
}