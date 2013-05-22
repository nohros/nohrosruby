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

    public void SetStatus(string service_name, Status status) {
      status_[service_name] = status;
    }

    public Status GetStatus(string service_name) {
      Status status;
      if (status_.TryGetValue(service_name, out status)) {
        return status;
      }
      return Status.Unknown;
    }
  }
}
