using System;
using System.Collections.Generic;
using Nohros.Concurrent;
using Nohros.Ruby.Protocol;
using ZMQ;
using ZmqContext = ZMQ.Context;
using ZmqSocket = ZMQ.Socket;
using ZmqSocketType = ZMQ.SocketType;

namespace Nohros.Ruby
{
  /// <summary>
  /// The communication channel that is used to comunnicate with a service
  /// tracker.
  /// </summary>
  public class TrackerMessageChannel : AbstractRubyMessageSender,
                                       IRubyMessageChannel, IDisposable
  {
    readonly ZmqContext context_;
    readonly ZMQEndPoint endpoint_;
    readonly RubyLogger logger_;
    readonly ZmqSocket socket_;
    bool opened_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="TrackerMessageChannel"/>
    /// class using the specified context and tracker endpoint address.
    /// </summary>
    /// <param name="context">
    /// A <see cref="ZmqContext"/> that can be used to create
    /// <see cref="Socket"/> objects.
    /// </param>
    /// <param name="endpoint">
    /// The tracker's address.
    /// </param>
    public TrackerMessageChannel(ZmqContext context, ZMQEndPoint endpoint,
      byte[] peer_id) {
      context_ = context;
      socket_ = context.Socket(ZmqSocketType.DEALER);
      socket_.Identity = peer_id;
      endpoint_ = endpoint;
      opened_ = false;
      logger_ = RubyLogger.ForCurrentProcess;
    }
    #endregion

    /// <inheritdoc/>
    public void Dispose() {
      socket_.Dispose();
    }

    /// <inheritdoc/>
    public override bool Send(RubyMessagePacket packet) {
      SendStatus status = socket_.Send(packet.ToByteArray(), SendRecvOpt.NOBLOCK);
      if (status == SendStatus.TryAgain) {
        if (!opened_) {
          throw new InvalidOperationException(
            Resources.InvalidOperation_ClosedChannel);
        }
        status = socket_.Send(packet.ToByteArray());
      }
      return status == SendStatus.Sent;
    }

    /// <summary>
    /// This method is not implemented for this channel.
    /// </summary>
    /// <remarks>
    /// A <see cref="TrackerMessageChannel"/> does not receive messages, it
    /// just send messages to the tracker. The tracker responses is received
    /// through another channel.
    /// </remarks>
    public void AddListener(IRubyMessagePacketListener listener, IExecutor executor)
    {
      return;
    }

    /// <inheritdoc/>
    public void Open() {
      opened_ = true;
      socket_.Connect(endpoint_.Endpoint);
    }

    /// <inheritdoc/>
    public void Close() {
      Close(0);
    }

    /// <inheritdoc/>
    public void Close(int timeout) {
      opened_ = false;
    }

    /// <summary>
    /// Gets the tracker's endpoint.
    /// </summary>
    public ZMQEndPoint Endpoint {
      get { return endpoint_; }
    }
  }
}
