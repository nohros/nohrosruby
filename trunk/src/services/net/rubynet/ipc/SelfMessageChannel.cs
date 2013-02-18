using System;
using System.Collections.Generic;
using Nohros.Ruby.Protocol.Control;
using R = Nohros.Resources;
using S = Nohros.Resources.StringResources;
using Nohros.Ruby.Protocol;
using Google.ProtocolBuffers;
using ZMQ;

namespace Nohros.Ruby
{
  internal class SelfMessageChannel : AbstractRubyMessageChannel,
                                      IRubyMessageChannel, IDisposable
  {
    const string kClassName = "Nohros.Ruby.SelfMessageChannel";
    readonly Socket channel_socket_;

    readonly Context context_;
    readonly IRubyLogger logger_;
    readonly string message_channel_endpoint_;
    readonly string tracker_channel_endpoint_;
    readonly Socket tracker_socket_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="SelfMessageChannel"/>
    /// class using the specified <see cref="Context"/> and channel
    /// endpoint address.
    /// </summary>
    /// <param name="context">
    /// A <see cref="Context"/> that can be used to create new sockets.
    /// </param>
    /// <param name="message_channel_endpoint">
    /// The address of the channel that is used to receive messages from the
    /// external world.
    /// </param>
    /// <param name="tracker_channel_endpoint">
    /// A <see cref="SelfChannelMessageHandler"/> object that can be used to
    /// process control messages.
    /// </param>
    public SelfMessageChannel(Context context, string message_channel_endpoint,
      string tracker_channel_endpoint) {
#if DEBUG
      if (context == null) {
        throw new ArgumentNullException("context");
      }

      if (message_channel_endpoint == null) {
        throw new ArgumentNullException("message_channel_endpoint")
      }

      if (tracker_channel_endpoint == null) {
        throw new ArgumentNullException("tracker_channel_endpoint");
      }
      
#endif
      context_ = context;
      logger_ = RubyLogger.ForCurrentProcess;
      channel_socket_ = context.Socket(SocketType.ROUTER);
      tracker_socket_ = context.Socket(SocketType.DEALER);
      message_channel_endpoint_ = message_channel_endpoint;
      tracker_channel_endpoint_ = tracker_channel_endpoint;
      ReplyTimeout = 30000;
    }
    #endregion

    /// <inheritdoc/>
    public void Dispose() {
      channel_socket_.Dispose();
    }

    /// <inheritdoc/>
    public override void Open() {
      // Open the socket before open the channel to ensure that the socket
      // is valid when GetMessagePacket is called.
      channel_socket_.Bind(Transport.TCP, message_channel_endpoint_);

      base.Open();
      if (logger_.IsDebugEnabled) {
        logger_.Debug("self message channel is opened at address: "
          + message_channel_endpoint_);
      }
    }

    /// <inheritdoc/>
    public override bool Send(RubyMessagePacket packet) {
      // The message envelope to receive a message over a REP/REQ->ROUTER
      // should be:
      //  [DESTINATION ADDRESS]
      //  [EMPTY FRAME]
      //  [DATA]
      try {
        RubyMessage message = packet.Message;

        // Filter control messages.
        switch (message.Type) {
          case (int) NodeMessageType.kNodeAnnounce:
          case (int) NodeMessageType.kNodeError:
          case (int) NodeMessageType.kNodeQuery:
            return SendControlMessage(packet);
        }

        // If the message could not be processed by the message handler, send
        // it over the communication channel.
        if (channel_socket_.SendMore(message.Sender.ToByteArray()) ==
          SendStatus.Sent) {
          if (channel_socket_.SendMore() == SendStatus.Sent) {
            if (channel_socket_.Send(packet.ToByteArray()) == SendStatus.Sent) {
              return true;
            }
          }
        }
      } catch (ZMQ.Exception e) {
        logger_.Error(string.Format(
          S.Log_MethodThrowsException, "Send", kClassName), e);
      }
      return false;
    }

    bool SendControlMessage(RubyMessagePacket packet) {
      Socket socket = context_.Socket(SocketType.REQ);
      socket.Connect(tracker_channel_endpoint_);

      // Send the request and wait for the response.
      return (socket.Send(packet.ToByteArray()) == SendStatus.Sent);
    }

    void SendAck(RubyMessage message) {
      if (message.AckType == RubyMessage.Types.AckType.kRubyNoAck) {
        return;
      }

      AckMessage ack_message =
        (message.AckType == RubyMessage.Types.AckType.kRubyRequestAck)
          ? new AckMessage.Builder().SetRequest(message).Build()
          : new AckMessage.Builder().Build();

      Send(message.Id.ToByteArray(), (int) NodeMessageType.kNodeAck,
        ack_message.ToByteArray(), message.Sender.ToByteArray());
    }

    /// <inheritdoc/>
    protected override RubyMessagePacket GetMessagePacket() {
      try {
        // The message envelope to receive a message over a REP/REQ->ROUTER
        // should be:
        //  [DESTINATION ADDRESS]
        //  [EMPTY FRAME]
        //  [DATA]
        Queue<byte[]> packets = channel_socket_.RecvAll();
        if (packets.Count%3 == 0) {
          byte[] sender = packets.Dequeue();
          packets.Dequeue(); // discard the empty frame

          RubyMessagePacket packet =
            RubyMessagePacket.ParseFrom(packets.Dequeue());

          // Associate the sender address with the received message.
          RubyMessage message = new RubyMessage.Builder(packet.Message)
            .SetSender(ByteString.CopyFrom(sender))
            .Build();

          SendAck(message);

          return new RubyMessagePacket.Builder(packet)
            .SetMessage(message)
            .Build();
        }
      } catch (ZMQ.Exception exception) {
        if (exception.Errno == (int) ERRNOS.ETERM) {
          logger_.Warn("The ZMQ context associated with the " +
            "self-message-channel was terminated. Closing the channel.");
          Close();
        }
      } catch (System.Exception exception) {
        logger_.Error(string.Format(R.StringResources.Log_MethodThrowsException,
          kClassName, "GetMessagePacket"), exception);
      }
      return new RubyMessagePacket.Builder().SetSize(0).Build();
    }

    public int ReplyTimeout { get; set; }
  }
}
