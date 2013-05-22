using System;

namespace Nohros.Ruby.Logging
{
  public struct Status
  {
    static readonly Status unknown_ = new Status {
      Timestamp = DateTime.MinValue,
      Type = StatusType.Unknown
    };

    public static Status Unknown {
      get { return unknown_; }
    }

    /// <summary>
    /// Gets or sets the date and time of the status.
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the status's type.
    /// </summary>
    public StatusType Type { get; set; }
  }
}
