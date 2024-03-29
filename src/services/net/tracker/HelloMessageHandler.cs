﻿using System;
using System.Collections.Generic;
using System.Net;
using Nohros.Ruby.Extensions;
using Nohros.Ruby.Protocol;
using Nohros.Ruby.Protocol.Control;
using ZMQ;
using Exception = System.Exception;
using R = Nohros.Resources.StringResources;

namespace Nohros.Ruby
{
  internal class HelloMessageHandler : IMessageHandler
  {
    const string kClassName = "Nohros.Ruby.HelloMessageHandler";

    readonly Broadcaster broadcaster_;
    readonly RubyLogger logger_;
    readonly IDictionary<string, Tracker> nodes_;
    readonly ITrackerFactory tracker_factory_;

    #region .ctor
    public HelloMessageHandler(IDictionary<string, Tracker> nodes,
      Broadcaster broadcaster, ITrackerFactory tracker_factory) {
      logger_ = RubyLogger.ForCurrentProcess;
      broadcaster_ = broadcaster;
      nodes_ = nodes;
      tracker_factory_ = tracker_factory;
    }
    #endregion

    public void Handle(IRubyMessage message) {
      try {
        HelloMessage hello = HelloMessage.ParseFrom(message.Message);
        var address = IPAddress.Parse(hello.Address);
        var endpoint = new IPEndPoint(address, hello.Port);
        string peer_id = message.Sender.AsBase64();

        // Sanity check if we know the id of the peer that is hosting our
        // service.
        if (broadcaster_.PeerID == null) {
          logger_.Warn("The ID of the service node is not know yet");
          return;
        }

        if (logger_.IsDebugEnabled) {
          logger_.Debug("The node associated with the "
            + "ID:  \"" + peer_id + "\" and located at endpoint: \""
            + endpoint.Address + ":" + endpoint.Port + "\" say HELLO");
        }

        // Connect back to the peer that say hello, if it was not done yet.
        Tracker node;
        if (!nodes_.TryGetValue(peer_id, out node) ||
          node.MessageChannel.Endpoint != endpoint) {
          node = tracker_factory_.CreateTracker(endpoint, Transport.TCP,
            broadcaster_.PeerID);
          node.MessageChannel.Open();
          nodes_[peer_id] = node;

          logger_.Info("connected to a new service node at: "
            + node.MessageChannel.Endpoint);
        }
        node.LastSeen = DateTime.Now;
      } catch (Exception e) {
        logger_.Error(
          string.Format(R.Log_MethodThrowsException, "OnHello", kClassName),
          e);
      }
    }
  }
}
