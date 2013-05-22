using System;

namespace Nohros.Ruby.Logging.Data
{
  public class Service
  {
    /// <summary>
    /// Gets a string that is used to uniquely identify a service.
    /// </summary>
    /// <remarks>
    /// This is the string that is used by a service to perform logging
    /// operations.
    /// </remarks>
    public string Name { get; set; }

    /// <summary>
    /// Gets a string that is used to friendly identify a service.
    /// </summary>
    public string DisplayName { get; set; }

    /// <summary>
    /// Gets a value that indicates how long a service should be idle before it
    /// is considered dead.
    /// </summary>
    public long MaxIdleTime { get; set; }
  }
}