using System;
using System.Collections.Generic;
using Nohros.Extensions;
using Nohros.Ruby.Extensions;
using Nohros.Ruby.Protocol;
using Nohros.Ruby.Protocol.Control;
using R = Nohros.Resources.StringResources;

namespace Nohros.Ruby
{
  public class QueryResponseMessageHandler : IMessageHandler
  {
    const string kClassName = "Nohros.Ruby.QueryResponseMessageHandler";

    readonly RubyLogger logger_;
    readonly IDictionary<int, Action<ZMQEndPoint>> queries_;

    #region .ctor
    public QueryResponseMessageHandler(
      IDictionary<int, Action<ZMQEndPoint>> queries) {
      queries_ = queries;
      logger_ = RubyLogger.ForCurrentProcess;
    }
    #endregion

    public void Handle(IRubyMessage message) {
      try {
        int id = message.Id.AsInt();
        ResponseMessage response = ResponseMessage.ParseFrom(message.Message);
        Action<ZMQEndPoint> callback;
        if (queries_.TryGetValue(id, out callback)) {
          lock (queries_) {
            // If the callback does not exists anymore, someone is already
            // processing the response.
            if (!queries_.Remove(id)) {
              return;
            }
          }
          foreach (var pair in response.AddressesList) {
            try {
              // TODO: Add the service to the local database
              callback(new ZMQEndPoint(pair));
            } catch (ArgumentException ae) {
              logger_.Error(string.Format(Resources.ZMQEndpoint_InvalidFormat,
                pair), ae);
            }
          }
        }
      } catch (Exception e) {
        logger_.Error(R.Log_MethodThrowsException.Fmt("OnQueryServiceResponse",
          kClassName), e);
      }
    }
  }
}
