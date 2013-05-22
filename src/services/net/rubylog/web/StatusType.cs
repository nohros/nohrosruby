using System;

namespace Nohros.Ruby.Logging
{
  public enum StatusType
  {
    /// <summary>
    /// Status used to inform about a informational log message.
    /// </summary>
    Info = 1,

    /// <summary>
    /// Status used to informa about a error log message.
    /// </summary>
    Error = 2,

    /// <summary>
    /// Status used to inform about a warning log message.
    /// </summary>
    Warn = 3,

    /// <summary>
    /// Status used to inform about an unknown status.
    /// </summary>
    /// <remarks>
    /// A service should be in that state until the first log message.
    /// </remarks>
    Unknown = 100
  }
}
