using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
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
  internal class SelfHostMessageChannel : AbstractRubyMessageSender,
                                          IRubyMessageChannel, IDisposable
  {
    public delegate void BeaconReceivedEventHandler(Beacon beacon);

    public delegate void MessagePacketReceivedEventHandler(
      RubyMessagePacket packet);

    const string kClassName = "Nohros.Ruby.SelfHostMessageChannel";
    const int kMaxBeaconPacketSize = 1024;

    readonly ZmqContext context_;
    readonly UdpClient discoverer_;
    readonly List<ListenerExecutorPair> listeners_;
    readonly RubyLogger logger_;
    readonly ZmqSocket receiver_;
    Thread discoverer_thread_;
    Thread mailbox_thread_;
    volatile bool opened_;
    ZMQEndPoint receiver_endpoint_;

    #region .ctor
    public SelfHostMessageChannel(ZmqContext context, UdpClient discoverer)
      : this(context, discoverer,
        new ZMQEndPoint(Strings.kDefaultSelfHostEndpoint)) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SelfHostMessageChannel"/>
    /// using the specified message channel and IPC sender and receive endpoint.
    /// </summary>
    /// <param name="receiver_endpoint">
    /// The address of the endpoint that is used to receive messages from
    /// peers.
    /// </param>
    /// <param name="discoverer">
    /// A <see cref="UdpClient"/> object that can be used to receive beacons
    /// from peers.
    /// </param>
    public SelfHostMessageChannel(ZmqContext context, UdpClient discoverer,
      ZMQEndPoint receiver_endpoint) {
      context_ = context;
      receiver_ = context_.Socket(ZmqSocketType.ROUTER);
      receiver_endpoint_ = receiver_endpoint;
      discoverer_ = discoverer;
      listeners_ = new List<ListenerExecutorPair>();
      logger_ = RubyLogger.ForCurrentProcess;
    }
    #endregion

    /// <inheritdoc/>
    public void Dispose() {
      Close();
      receiver_.Dispose();
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

        // TODO: Find a way to wake up the discoverer Thread.
        //
        var timer = new Stopwatch();
        timer.Start();
        timer.Stop();

        // Compute the remaining time that we have to wait for the mailbox
        // thread to finish.
        int remaining_timeout = timeout - (int) timer.Elapsed.TotalSeconds;
        if (remaining_timeout > 0) {
          mailbox_thread_.Join(TimeSpan.FromSeconds(remaining_timeout));
        }
      }
    }

    /// <inheritdoc/>
    public void AddListener(IRubyMessageListener listener, IExecutor executor) {
      listeners_.Add(new ListenerExecutorPair(listener, executor));
    }

    /// <inheritdoc/>
    public void Open() {
      opened_ = true;
      BindReceiverSocket();
      discoverer_thread_ = new BackgroundThreadFactory()
        .CreateThread(DiscovererThread);
      mailbox_thread_ = new BackgroundThreadFactory()
        .CreateThread(MailboxThread);
      discoverer_thread_.Start();
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

    void BindReceiverSocket() {
      // If receiver endpoint port is specified as 0, binds to any free port
      // from kMinEphemeralPort to kMinEphemeralPort
      if (receiver_endpoint_.Port == 0) {
        int port = ZMQEndPoint.kMinEphemeralPort;
        string endpoint_suffix = receiver_endpoint_.Transport.AsString()
          + "://" + receiver_endpoint_.Address + ":";
        while (port < ZMQEndPoint.kMaxEphemeralPort) {
          try {
            string endpoint = endpoint_suffix + port.ToString();
            receiver_.Bind(endpoint);
            receiver_endpoint_ = new ZMQEndPoint(endpoint);
            return;
          } catch {
          }
        }
      }
      receiver_.Bind(receiver_endpoint_.ToString());
    }

    /// <summary>
    /// Raised when a <see cref="RubyMessagePacket"/> is received in the
    /// <see cref="SelfHostMessageChannel"/> mailbox.
    /// </summary>
    /// <remarks>
    /// This event is not raised when a message is received through the service
    /// message channel. If you want to receive this events register a listener
    /// using the <see cref="AddListener"/> method.
    /// </remarks>
    public event MessagePacketReceivedEventHandler MessagePacketReceived;

    /// <summary>
    /// Raised when a <see cref="RubyMessagePacket"/> is sent over the channel.
    /// </summary>
    public event MessagePacketReceivedEventHandler MessagePacketSent;

    /// <summary>
    /// Raised when a beacon is received.
    /// </summary>
    public event BeaconReceivedEventHandler BeaconReceived;

    void OnBeaconReceived(Beacon beacon) {
      Listeners.SafeInvoke<BeaconReceivedEventHandler>(BeaconReceived,
        handler => handler(beacon));
    }

    void OnMessagePacketReceived(RubyMessagePacket packet) {
      Listeners
        .SafeInvoke<MessagePacketReceivedEventHandler>(MessagePacketReceived,
          handler => handler(packet));
    }

    void OnMessagePacketSent(RubyMessagePacket packet) {
      Listeners
        .SafeInvoke<MessagePacketReceivedEventHandler>(MessagePacketSent,
          handler => handler(packet));
    }

    bool Reply(RubyMessagePacket packet) {
      receiver_.SendMore(packet.Message.Sender.ToByteArray());
      SendStatus status = receiver_.Send(packet.ToByteArray(),
        SendRecvOpt.NOBLOCK);
      if (status == SendStatus.TryAgain) {
        if (!opened_) {
          throw new InvalidOperationException(
            Resources.InvalidOperation_ClosedChannel);
        }
        status = receiver_.Send(packet.ToByteArray());
      }
      return status == SendStatus.Sent;
    }

    void DiscovererThread() {
      var peer_endpoint = new IPEndPoint(IPAddress.Any, 0);
      while (opened_) {
        try {
          var beacon = discoverer_.Receive(ref peer_endpoint);
          if (beacon.Length > 0) {
            OnBeaconReceived(Beacon.FromByteArray(beacon, peer_endpoint.Address));
          }
        } catch (SocketException e) {
          // If the socket has been shutdown finish the thread.
          if (e.SocketErrorCode == SocketError.Shutdown) {
            return;
          }
          logger_.Error(string.Format(
            R.Log_MethodThrowsException, "DiscovererThread", kClassName), e);
        }
      }
    }

    void MailboxThread() {
      while (opened_) {
        try {
          Queue<byte[]> parts = receiver_.RecvAll();
          if (parts.Count != 2) {
            if (logger_.IsWarnEnabled) {
              logger_.Warn("");
            }
          }
          OnMessagePacketReceived(GetRubyMessagePacket(parts));
        } catch (Exception e) {
          logger_.Error(string.Format(R.Log_MethodThrowsException,
            "MailboxThread", kClassName), e);
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
  }
}
