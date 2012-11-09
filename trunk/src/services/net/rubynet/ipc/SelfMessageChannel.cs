using System;
using System.Collections.Generic;
using R = Nohros.Resources;
using Nohros.Ruby.Protocol;
using Google.ProtocolBuffers;
using ZMQ;

namespace Nohros.Ruby
{
  internal class SelfMessageChannel : AbstractRubyMessageChannel,
                                      IRubyMessageChannel, IDisposable
  {
    const string kClassName = "Nohros.Ruby.SelfMessageChannel";

    readonly Context context_;
    readonly IRubyLogger logger_;
    readonly string message_channel_endpoint_;
    readonly Socket socket_;

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
    public SelfMessageChannel(Context context, string message_channel_endpoint) {
#if DEBUG
      if (context == null || message_channel_endpoint == null) {
        throw new ArgumentNullException(context == null
          ? "socket"
          : "string message_channel_endpoint");
      }
#endif
      context_ = context;
      socket_ = context.Socket(SocketType.ROUTER);
      message_channel_endpoint_ = message_channel_endpoint;
      logger_ = RubyLogger.ForCurrentProcess;
    }
    #endregion

    /// <inheritdoc/>
    public void Dispose() {
      socket_.Dispose();
    }

    /// <inheritdoc/>
    public override void Open() {
      // Open the socket before open the channel to ensure that the socket
      // is valid when GetMessagePacket is called.
      socket_.Bind(Transport.TCP, message_channel_endpoint_);
      base.Open();
      if (logger_.IsDebugEnabled) {
        logger_.Debug("self-message-channel is opened.");
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
        if (socket_.SendMore(message.Sender.ToByteArray()) == SendStatus.Sent) {
          if (socket_.SendMore() == SendStatus.Sent) {
            if (socket_.Send(packet.ToByteArray()) == SendStatus.Sent) {
              return true;
            }
          }
        }
      } catch (ZMQ.Exception) {
        // TODO: Add logging
      }
      return false;
    }

    /// <inheritdoc/>
    protected override RubyMessagePacket GetMessagePacket() {
      try {
        // The message envelope to receive a message over a REP/REQ->ROUTER
        // should be:
        //  [DESTINATION ADDRESS]
        //  [EMPTY FRAME]
        //  [DATA]
        Queue<byte[]> packets = socket_.RecvAll();
        if (packets.Count%3 == 0) {
          byte[] sender = packets.Dequeue();
          packets.Dequeue(); // discard the empty frame

          RubyMessagePacket packet =
            RubyMessagePacket.ParseFrom(packets.Dequeue());

          // Associate the sender address with the received message.
          RubyMessage message = new RubyMessage.Builder(packet.Message)
            .SetSender(ByteString.CopyFrom(sender))
            .Build();
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
  }
}
