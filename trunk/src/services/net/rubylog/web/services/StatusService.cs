using System;
using Nohros.Data.Json;
using ServiceStack.ServiceInterface;

namespace Nohros.Ruby.Logging
{
  public class StatusService : Service
  {
    readonly StatusManager status_manager_;

    #region .ctor
    public StatusService(StatusManager status_manager) {
      status_manager_ = status_manager;
    }
    #endregion

    public string Get(StatusRequest request) {
      Status status = status_manager_.GetStatus(request.ServiceName);
      return new JsonStringBuilder()
        .WriteBeginObject()
        .WriteMember("timestamp", TimeUnitHelper.ToUnixTime(status.Timestamp))
        .WriteMember("type", (int) status.Type)
        .WriteEndObject()
        .ToString();
    }
  }
}
