using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using System.Collections.Specialized;
using System.Threading;

using MyToolsPack.Console;

using Nohros.Logging;
using Nohros.Desktop;

namespace Nohros.Ruby.Service.Net
{
  internal class RubyShell : IMyToolsPackConsole
  {
    const string kAssemblyNameSwitch = "assembly";
    const string kTypeNameSwitch = "type";

    const string kShellPrompt = "rubynet$: ";
    const string kExitCommand = "exit";

    CommandLine command_line_;
    RubySettings settings_;
    RubyServiceHosts service_hosts_;
    MyToolsPackConsole console_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="RubyNetShell"/> by using
    /// the specified <see cref="CommandLine"/> and <see cref="RubySettings"/>
    /// </summary>
    public RubyShell(RubySettings settings,
      MyToolsPackConsole console) : base() {
      service_hosts_ = new RubyServiceHosts(10);
      settings_ = settings;
      console_ = console;
    }
    #endregion

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
    public void Run() {
      console_.Run();
    }

    /// <inheritdoc/>
    void IMyToolsPackConsole.LoadCommand(string nspace, string command_name,
      ICommandFactory factory) {
        console_.LoadCommand(nspace, command_name, factory);
    }

    /// <inheritdoc/>
    public void Write(string message) {
      console_.Write(message);
    }

    /// <inheritdoc/>
    public void WriteLine(string message) {
      console_.WriteLine(message);
    }

    /// <summary>
    /// Gets the list of hosts that currently runnning a service.
    /// </summary>
    internal RubyServiceHosts ServiceHosts {
      get { return service_hosts_; }
    }
  }
}