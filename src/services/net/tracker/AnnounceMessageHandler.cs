using System;
using System.Collections.Generic;
using Nohros.Ruby.Data;
using Nohros.Ruby.Extensions;
using Nohros.Ruby.Protocol;
using Nohros.Ruby.Protocol.Control;
using R = Nohros.Resources.StringResources;

namespace Nohros.Ruby
{
  internal class AnnounceMessageHandler : IMessageHandler
  {
    const string kClassName = "Nohros.Ruby.AnnounceMessageHandler";

    readonly RubyLogger logger_;
    readonly IDictionary<string, Tracker> nodes_;
    readonly IServicesRepository services_repository_;

    #region .ctor
    public AnnounceMessageHandler(IDictionary<string, Tracker> nodes,
      IServicesRepository services_repository) {
      services_repository_ = services_repository;
      logger_ = RubyLogger.ForCurrentProcess;
      nodes_ = nodes;
    }
    #endregion

    public void Handle(IRubyMessage message) {
      Tracker tracker;
      if (!nodes_.TryGetValue(message.Sender.AsBase64(), out tracker)) {
        // If we do not have information about the message sender, discard
        // the message.
        logger_.Warn("received a message from a unknown node. Node ID: "
          + message.Sender.AsBase64());
        return;
      }
      try {
        AnnounceMessage announcement = AnnounceMessage.ParseFrom(message.Message);
        INewServiceCommand command;
        services_repository_.Query(out command)
          .SetEndPoint(tracker.MessageChannel.Endpoint)
          .SetFacts(new ServiceFacts(announcement.FactsList.ToKeyValuePairs()))
          .Execute();
      } catch (Exception e) {
        logger_.Error(string.Format(R.Log_MethodThrowsException, "Handle",
          kClassName), e);
      }
    }
  }
}
