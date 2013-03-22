using System;
using System.Collections.Generic;
using System.Threading;
using Google.ProtocolBuffers;
using Nohros.Concurrent;
using Nohros.Ruby.Extensions;
using Nohros.Ruby.Protocol;
using ZMQ;
using Exception = System.Exception;
using ZmqSocketType = ZMQ.SocketType;
using ZmqSocket = ZMQ.Socket;
using ZmqContext = ZMQ.Context;
using R = Nohros.Resources.StringResources;

namespace Nohros.Ruby
{
  /// <summary>
  /// A implementation of the <see cref="IRubyMessageChannel"/> class that
  /// provides self hosting capabilities.
  /// </summary>
  /// <remarks>
  /// This class is intendeed to be used in enviroments where a service host
  /// node is not required (ex. communicate with a service from a web server).
  /// </remarks>
  internal class HostMessageChannel : MultiplexedMessageChannel,
                                      IRubyMessageChannel, IDisposable
  {
    public delegate void MailboxBindEventHandler(ZMQEndPoint endpoint);

    public delegate void MessagePacketReceivedEventHandler(
      RubyMessagePacket packet);

    const string kClassName = "Nohros.Ruby.HostMessageChannel";
    const string kSenderChannelEndpoint = "inproc://ruby-multiplex-self-channel";

    readonly ZmqContext context_;
    readonly List<ListenerExecutorPair> listeners_;
    readonly RubyLogger logger_;
    Thread mailbox_thread_;
    volatile bool opened_;
    ZMQEndPoint receiver_endpoint_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="HostMessageChannel"/>
    /// using the specified message channel and IPC sender and receive endpoint.
    /// </summary>
    /// <param name="receiver_endpoint">
    /// The address of the endpoint that is used to receive messages from
    /// peers.
    /// </param>
    public HostMessageChannel(ZmqContext context, ZMQEndPoint receiver_endpoint) {
      context_ = context;
      receiver_endpoint_ = receiver_endpoint;
      listeners_ = new List<ListenerExecutorPair>();
      logger_ = RubyLogger.ForCurrentProcess;
    }
    #endregion

    /// <inheritdoc/>
    public void Dispose() {
      Close();
    }

    /// <inheritdoc/>
    public void Close() {
      Close(0);
    }

    /// <inheritdoc/>
    public void Close(int timeout) {
      // Closes the channel only if it is opened.
      if (opened_) {
        opened_ = false;

        // TODO (neylor.silva): Find a way to wake up the discoverer and mailbox
        // Thread.
        //
        mailbox_thread_.Join(TimeSpan.FromSeconds(timeout));
      }
      logger_.Info("self host channel has been closed.");
    }

    /// <inheritdoc/>
    public void AddListener(IRubyMessagePacketListener listener, IExecutor executor) {
      listeners_.Add(new ListenerExecutorPair(listener, executor));
    }

    /// <inheritdoc/>
    public void Open() {
      opened_ = true;
      mailbox_thread_ = new BackgroundThreadFactory()
        .CreateThread(Multiplex);
      mailbox_thread_.Start();
    }

    /// <inheritdoc/>
    public override bool Send(RubyMessagePacket packet) {
      // |packet| represents a message sent from a service and could could be a
      // reply or a request. If it is a reply the sender id should be set;
      // otherwise it will be considered as a request and will be delivered
      // to one of the connected trackers.
      if (packet.Message.HasSender) {
        return Reply(packet);
      }
      OnMessagePacketSent(packet);
      return true;
    }

    ZMQEndPoint Bind(ZmqSocket socket, ZMQEndPoint endpoint) {
      // If endpoint port is specified as 0, binds to any free port
      // from kMinEphemeralPort to kMaxEphemeralPort
      if (endpoint.Port == 0) {
        int port = ZMQEndPoint.kMinEphemeralPort;
        string endpoint_suffix = endpoint.Transport.AsString()
          + "://" + endpoint.Address + ":";
        while (port++ < ZMQEndPoint.kMaxEphemeralPort) {
          try {
            string address = endpoint_suffix + port.ToString();
            socket.Bind(address);
            return new ZMQEndPoint(address);
          } catch {
          }
        }
      }
      socket.Bind(receiver_endpoint_.ToString());
      return endpoint;
    }

    /// <summary>
    /// Raised when a <see cref="RubyMessagePacket"/> is received in the
    /// <see cref="HostMessageChannel"/> mailbox.
    /// </summary>
    /// <remarks>
    /// This event is not raised when a message is received through the service
    /// message channel. If you want to receive this events register a listener
    /// using the <see cref="AddListener"/> method.
    /// </remarks>
    public event MessagePacketReceivedEventHandler MailboxMessagePacketReceived;

    public event MailboxBindEventHandler MailboxBind;

    public void OnMailboxBind(ZMQEndPoint endpoint) {
      Listeners.SafeInvoke<MailboxBindEventHandler>(MailboxBind,
        handler => handler(endpoint));
    }

    /// <summary>
    /// Raised when a <see cref="RubyMessagePacket"/> is sent over the channel.
    /// </summary>
    public event MessagePacketReceivedEventHandler MessagePacketSent;

    public void OnMessagePacketReceived(RubyMessagePacket packet) {
      for (int i = 0, j = listeners_.Count; i < j; i++) {
        ListenerExecutorPair pair = listeners_[i];
        pair.Executor.Execute(
          () => pair.Listener.OnMessagePacketReceived(packet));
      }
    }

    /// <summary>
    /// Raises the <see cref="MailboxMessagePacketReceived"/> event.
    /// </summary>
    /// <param name="packet">
    /// As <see cref="RubyMessagePacket"/> containing the received packet.
    /// </param>
    public void OnMailboxMessagePacketReceived(RubyMessagePacket packet) {
      Listeners
        .SafeInvoke<MessagePacketReceivedEventHandler>(
          MailboxMessagePacketReceived,
          handler => handler(packet));
    }

    void OnMessagePacketSent(RubyMessagePacket packet) {
      Listeners
        .SafeInvoke<MessagePacketReceivedEventHandler>(MessagePacketSent,
          handler => handler(packet));
    }

    bool Reply(RubyMessagePacket packet) {
      using (ZmqSocket socket = context_.Socket(ZmqSocketType.REQ)) {
        socket.Connect(kSenderChannelEndpoint);
        socket.SendMore(packet.Message.Sender.ToByteArray());
        SendStatus status = socket.Send(packet.ToByteArray(),
          SendRecvOpt.NOBLOCK);
        if (status == SendStatus.TryAgain) {
          if (!opened_) {
            throw new InvalidOperationException(
              Resources.InvalidOperation_ClosedChannel);
          }
          status = socket.Send(packet.ToByteArray());
        }
        return status == SendStatus.Sent;
      }
    }

    void OnMessageReceived(ZmqSocket socket) {
      try {
        Queue<byte[]> parts = socket.RecvAll();
        if (parts.Count != 2) {
          if (logger_.IsWarnEnabled) {
            logger_.Warn("");
          }
        }
        OnMailboxMessagePacketReceived(GetRubyMessagePacket(parts));
      } catch (Exception e) {
        logger_.Error(string.Format(R.Log_MethodThrowsException,
          "MailboxThread", kClassName), e);
      }
    }

    void Multiplex() {
      var inproc_socket = context_.Socket(ZmqSocketType.REP);
      inproc_socket.Bind(kSenderChannelEndpoint);

      var mailbox_socket = context_.Socket(ZmqSocketType.ROUTER);
      receiver_endpoint_ = Bind(mailbox_socket, receiver_endpoint_);

      OnMailboxBind(receiver_endpoint_);

      logger_.Info("self host mailbox bound to port: \""
        + receiver_endpoint_.Port + "\"");

      var items = new[] {
        inproc_socket.CreatePollItem(IOMultiPlex.POLLIN),
        mailbox_socket.CreatePollItem(IOMultiPlex.POLLIN)
      };
      items[0].PollInHandler +=
        (socket, revents) => Proxy(socket, mailbox_socket);
      items[1].PollInHandler += (socket, revents) => OnMessageReceived(socket);

      while (opened_) {
        try {
          context_.Poll(items);
        } catch (ZMQ.Exception e) {
          // Prevent the normal context termination to raise an exception.
          if (e.Errno == (int) ERRNOS.ETERM) {
            return;
          }
          throw;
        }
      }
    }

    RubyMessagePacket GetRubyMessagePacket(Queue<byte[]> parts) {
      byte[] sender_id = parts.Dequeue();
      RubyMessagePacket packet = RubyMessagePacket.ParseFrom(parts.Dequeue());
      RubyMessage message = RubyMessage.CreateBuilder(packet.Message)
        .SetSender(ByteString.CopyFrom(sender_id))
        .Build();
      return RubyMessagePacket.CreateBuilder(packet)
        .SetMessage(message)
        .Build();
    }

    public ZMQEndPoint Endpoint {
      get { return receiver_endpoint_; }
    }
  }
}
