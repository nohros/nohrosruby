using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Nohros.Concurrent;
using Nohros.Ruby.Protocol;
using System.Net.Sockets;
using Socket = System.Net.Sockets.Socket;
using ZmqSocketType = ZMQ.SocketType;
using ZmqSocket = ZMQ.Socket;
using ZmqContext = ZMQ.Context;
using R = Nohros.Resources.StringResources;

namespace Nohros.Ruby
{
  internal class SelfHostMessageChannel : IRubyMessageChannel, IDisposable
  {
    public delegate void BeaconReceivedEventHandler(Beacon beacon);

    const string kClassName = "Nohros.Ruby.SelfHostMessageChannel";
    const int kMaxBeaconPacketSize = 1024;

    readonly ZmqContext context_;
    readonly UdpClient discoverer_;
    readonly string endpoint_;
    readonly RubyLogger logger_;
    readonly IRubyMessageChannel ruby_message_channel_;
    readonly ZmqSocket socket_;
    Thread discoverer_thread_;
    Thread mailbox_thread_;
    volatile bool opened_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="SelfHostMessageChannel"/>
    /// using the specified message channel and IPC sender and receive endpoint.
    /// </summary>
    /// <param name="ruby_message_channel">
    /// A <see cref="IRubyMessageChannel"/> that can be used to foward the
    /// <see cref="IRubyMessageChannel"/> method calls.
    /// </param>
    /// <param name="endpoint">
    /// The address of the endpoint that is used to receive messages from
    /// peers.
    /// </param>
    /// <param name="discoverer">
    /// A <see cref="UdpClient"/> object that can be used to receive beacons
    /// from peers.
    /// </param>
    public SelfHostMessageChannel(IRubyMessageChannel ruby_message_channel,
      ZmqContext context, string endpoint, UdpClient discoverer) {
      ruby_message_channel_ = ruby_message_channel;
      context_ = context;
      socket_ = context_.Socket(ZmqSocketType.ROUTER);
      endpoint_ = endpoint;
      discoverer_ = discoverer;
      logger_ = RubyLogger.ForCurrentProcess;
    }
    #endregion

    /// <inheritdoc/>
    public void Dispose() {
      Close();
      socket_.Dispose();
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
        ruby_message_channel_.Close(timeout);
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
      ruby_message_channel_.AddListener(listener, executor);
    }

    /// <inheritdoc/>
    public void Open() {
      opened_ = true;
      socket_.Bind(endpoint_);
      ruby_message_channel_.Open();
      discoverer_thread_ = new BackgroundThreadFactory()
        .CreateThread(DiscovererThread);
      mailbox_thread_ = new BackgroundThreadFactory()
        .CreateThread(MailboxThread);
      discoverer_thread_.Start();
      mailbox_thread_.Start();
    }

    /// <inheritdoc/>
    public bool Send(IRubyMessage message) {
      return ruby_message_channel_.Send(message);
    }

    /// <inheritdoc/>
    public bool Send(RubyMessagePacket packet) {
      return ruby_message_channel_.Send(packet);
    }

    /// <inheritdoc/>
    public bool Send(byte[] message_id, int type, byte[] message,
      byte[] destination, string token,
      IEnumerable<KeyValuePair<string, string>> facts) {
      return ruby_message_channel_.Send(message_id, type, message, destination,
        token, facts);
    }

    /// <inheritdoc/>
    public bool Send(byte[] message_id, int type, byte[] message,
      byte[] destination) {
      return ruby_message_channel_.Send(message_id, type, message, destination);
    }

    /// <inheritdoc/>
    public bool Send(IRubyMessage message,
      IEnumerable<KeyValuePair<string, string>> facts) {
      return ruby_message_channel_.Send(message, facts);
    }

    /// <inheritdoc/>
    public bool Send(byte[] message_id, int type, byte[] message,
      byte[] destination, string token) {
      return ruby_message_channel_.Send(message_id, type, message, destination,
        token);
    }

    /// <inheritdoc/>
    public bool Send(byte[] message_id, int type, byte[] message,
      byte[] destination, IEnumerable<KeyValuePair<string, string>> facts) {
      return ruby_message_channel_.Send(message_id, type, message, destination,
        facts);
    }

    /// <summary>
    /// Raised when a beacon is received.
    /// </summary>
    public event BeaconReceivedEventHandler BeaconReceived;

    void OnBeaconReceived(Beacon beacon) {
      Listeners.SafeInvoke<BeaconReceivedEventHandler>(BeaconReceived,
        handler => handler(beacon));
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
      }
    }
  }
}
