using System;
using Nohros.MyToolsPack.Console;

namespace Nohros.Ruby
{
  internal class ShellRubyProcess : AbstractRubyProcess, IMyToolsPackConsole,
                                    IRubyProcess
  {
    readonly IMyToolsPackConsole console_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="ShellRubyProcess"/> class by
    /// using the specified <see cref="CommandLine"/> and
    /// <see cref="RubySettings"/> objects.
    /// </summary>
    public ShellRubyProcess(IMyToolsPackConsole console,
      IRubySettings settings,
      IRubyMessageChannel ruby_message_channel)
      : base(settings, ruby_message_channel) {
      console_ = console;
    }
    #endregion

    /// <inheritdoc/>
    void IMyToolsPackConsole.LoadCommand(string nspace, string command_name,
      ICommandFactory factory) {
      console_.LoadCommand(nspace, command_name, factory);
    }

    /// <inheritdoc/>
    void IMyToolsPackConsole.SetOption(string key, string value) {
      console_.SetOption(key, value);
    }

    /// <inheritdoc/>
    public void Write(string message) {
      console_.Write(message);
    }

    /// <inheritdoc/>
    public void WriteLine(string message) {
      console_.WriteLine(message);
    }

    /// <inheritdoc/>
    public override void Run(string command_line_string) {
      // A try-block is used to catch any unhandled exception that is raised
      // by a service.
      try {
        base.Run(command_line_string);
        QueryLogAggregatorService();
        console_.Run(command_line_string);
        Exit();
      } catch (Exception ex) {
        string message = "";
        Exception exception = ex;
        while (exception != null) {
          // Is unusual to have a great number of inner excpetions and this
          // piece of code does not impact the application performance, so
          // using a string concatenation is OK.
          message += exception.Message;
          exception = exception.InnerException;
        }
        RubyLogger.ForCurrentProcess.Error(message);
      }
    }
  }
}
