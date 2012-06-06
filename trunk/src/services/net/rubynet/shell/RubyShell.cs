using System;
using System.IO.Pipes;

using Nohros.Desktop;
using Nohros.MyToolsPack.Console;

namespace Nohros.Ruby.Service.Net
{
  internal class RubyShell : IMyToolsPackConsole
  {
    readonly IMyToolsPackConsole console_;
    readonly RubyServiceHosts service_hosts_;
    readonly RubySettings settings_;
    readonly NamedPipeServerStream ipc_channel_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="RubyShell"/> class by
    /// using the specified <see cref="CommandLine"/> and
    /// <see cref="RubySettings"/> objects.
    /// </summary>
    public RubyShell(RubySettings settings, IMyToolsPackConsole console)
      : this(settings, console, null) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RubyShell"/> class by
    /// using the specified command line, shell console and IPC channel.
    /// </summary>
    public RubyShell(RubySettings settings, IMyToolsPackConsole console,
      NamedPipeServerStream ipc_channel) {
      service_hosts_ = new RubyServiceHosts(10);
      settings_ = settings;
      console_ = console;
      ipc_channel_ = ipc_channel;
    }
    #endregion

    #region IMyToolsPackConsole Members
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
    #endregion

    /// <summary>
    /// Runs the shell.
    /// </summary>
    public void Run() {
      Run(string.Empty);
    }

    /// <summary>
    /// Runs the shell.
    /// </summary>
    /// <remarks>
    /// Legacy version of this app was used to start a single service without
    /// a shell. For compatbility with this legacy softwares this method allows
    /// a string to be specified as a argument, this string represents the list
    /// of arguments that is accepted by the start command. When supplied the
    /// shell will run and the specified service will be started; after that
    /// the shell will runs normally.
    /// </remarks>
    public void Run(string command_line_string) {
      // A try-block is used to catch any unhandled exception that is raised
      // by a service.
      try {
        console_.Run(command_line_string);
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

    /// <summary>
    /// Gets the list of hosts that is currently runnning a service.
    /// </summary>
    public RubyServiceHosts ServiceHosts {
      get { return service_hosts_; }
    }

    /// <summary>
    /// Gets a <see cref="NamedPipeServerStream"/> object that is used to
    /// communicate with the service node.
    /// </summary>
    /// <remarks>
    /// This property is only valid only when we are running without a OS
    /// shell attached(service-mode). When a OS shell is attached, this
    /// property return <c>null</c>.
    /// </remarks>
    public NamedPipeServerStream IPCChannel {
      get { return ipc_channel_; }
    }
  }
}
