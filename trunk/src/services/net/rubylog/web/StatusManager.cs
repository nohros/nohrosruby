using System;
using System.Collections.Generic;

namespace Nohros.Ruby.Logging
{
  public class StatusManager
  {
    readonly IDictionary<string, Status> status_;

    #region .ctor
    public StatusManager() {
      status_ = new Dictionary<string, Status>(10);
    }
    #endregion

    /// <summary>
    /// Sets the status of the service named <paramref name="service_name"/>.
    /// </summary>
    /// <param name="service_name">
    /// The name of the service to set the status.
    /// </param>
    /// <param name="status">
    /// The current status of the service.
    /// </param>
    public void SetStatus(string service_name, Status status) {
      status_[service_name] = status;
    }

    /// <summary>
    /// Gets the current status of the service named
    /// <paramref name="service_name"/>.
    /// </summary>
    /// <param name="service_name">
    /// The name of the service to get the status.
    /// </param>
    /// <returns>
    /// If a service with name <paramref name="service_name"/> is not found
    /// a <see cref="Status.Unknown"/> is returned.
    /// </returns>
    public Status GetStatus(string service_name) {
      Status status;
      if (status_.TryGetValue(service_name, out status)) {
        return status;
      }
      return Status.Unknown;
    }
  }
}
