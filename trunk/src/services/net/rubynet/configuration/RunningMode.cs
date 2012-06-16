using System;

namespace Nohros.Ruby
{
  public enum RunningMode
  {
    /// <summary>
    /// The application is running in service mode.
    /// </summary>
    /// <remarks>
    /// In service mode the shell is not displayed and the clients can
    /// communicate with the services only throught the ruby server.
    /// </remarks>
    Service = 1,

    /// <summary>
    /// The application is running in the interactive mode.
    /// </summary>
    /// <remarks>
    /// When running in the interactive mode a shell is displayed to the user.
    /// This shell could be used to handle the service communication. Note
    /// that communication throught ruby server works too.
    /// </remarks>
    Interactive = 2
  }
}
