using System;

namespace Nohros.Ruby.Shell
{
  /// <summary>
  /// Stops a running service.
  /// </summary>
  internal class StopCommand: ShellCommand
  {
    readonly string service_name_;

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
    public override void Run(ShellRubyProcess process) {
      process.UnhostService(service_name_);
    }
  }
}