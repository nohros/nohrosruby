using System;
using System.Collections.Generic;
using System.Text;

using Nohros.Desktop;

namespace Nohros.Ruby.Service.Net
{
  /// <summary>
  /// Stops a running service.
  /// </summary>
  internal class StopCommand: ShellCommand
  {
    string service_name_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="StopCommand"/> by using
    /// the specified command name.
    /// </summary>
    /// <param name="service_name">The name of the service to stop.</param>
    public StopCommand(string service_name)
      : base(ShellSwitches.kStopCommand) {
#if DEBUG
      if (service_name == null)
        throw new ArgumentNullException("service_name");

      if (service_name.Length == 0)
        throw new ArgumentException("service_name");
#endif
      service_name_ = service_name;
    }
    #endregion

    /// <summary>
    /// Runs the stop command.
    /// </summary>
    public override void Run(RubyShell shell) {
      RubyServiceHosts hosts = shell.ServiceHosts;
      hosts.UnhostService(service_name_);
    }
  }
}