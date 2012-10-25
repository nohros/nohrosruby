using System;

namespace Nohros.Ruby.Shell
{
  internal sealed class ShellStrings
  {
    #region .ctor
    ShellStrings() { }
    #endregion

    /// <summary>
    /// Start a new service.
    /// </summary>
    public const string kStartCommand = "start";

    /// <summary>
    /// Stops a running service.
    /// </summary>
    public const string kStopCommand = "stop";


    public const string kSendCommand = "sent";

    /// <summary>
    /// Clear the console window.
    /// </summary>
    public const string kClearCommand = "clear";

    /// <summary>
    /// Exits the console.s
    /// </summary>
    public const string kExitCommand = "exit";

    /// <summary>
    /// Show the shell help.
    /// </summary>
    public const string kHelpCommand = "help";

    /// <summary>
    /// Launches and attaches a debugger to the process.s
    /// </summary>
    public const string kDebugCommand = "debug";

    /// <summary>
    /// Start the process with a interactive shell. This flag could not be used
    /// when the service is running in service mode.
    /// </summary>
    public const string kWithShell = "with-shell";

    /// <summary>
    /// Specifies the path of the service assembly.
    /// </summary>
    public const string kAssemblyNameSwitch = "assembly";

    /// <summary>
    /// Specifies the assembly's fully qualified name of the service class
    /// factory.
    /// </summary>
    public const string kTypeNameSwitch = "type";

    /// <summary>
    /// Specifies the name of the pipe that will be used for IPC communication.
    /// </summary>
    public const string kPipeSwitch = "pipe";

    /// <summary>
    /// Specifies the name of the service.
    /// </summary>
    public const string kService = "service";

    public const string kShellServiceName = "shell";
  }
}
