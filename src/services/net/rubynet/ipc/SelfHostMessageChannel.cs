using System;
using System.Collections.Generic;
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

namespace Nohros.Ruby
{
  internal class SelfHostMessageChannel : IRubyMessageChannel
  {
    public delegate void BeaconMessageHandler(byte[] beacon);

    readonly int broadcast_port_;

    readonly ZmqContext context_;
    readonly Socket discoverer_;
    readonly string endpoint_;
    readonly IRubyMessageChannel ruby_message_channel_;
    readonly ZmqSocket socket_;
    Thread discoverer_thread_;

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
    public SelfHostMessageChannel(IRubyMessageChannel ruby_message_channel,
      ZmqContext context, string endpoint, int broadcast_port) {
      ruby_message_channel_ = ruby_message_channel;
      context_ = context;
      socket_ = context_.Socket(ZmqSocketType.ROUTER);
      endpoint_ = endpoint;
      discoverer_ = new Socket(AddressFamily.InterNetwork, SocketType.Dgram,
        ProtocolType.Udp);
      broadcast_port_ = broadcast_port;
    }
    #endregion

    /// <inheritdoc/>
    public void AddListener(IRubyMessageListener listener, IExecutor executor) {
      ruby_message_channel_.AddListener(listener, executor);
    }

    /// <inheritdoc/>
    public void Open() {
      socket_.Bind(endpoint_);
      discoverer_.Bind(new IPEndPoint(IPAddress.Any, broadcast_port_));
      discoverer_thread_ = new BackgroundThreadFactory()
        .CreateThread(DiscovererThread);
      discoverer_thread_.Start();
      ruby_message_channel_.Open();
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
    public event BeaconMessageHandler BeaconMessageReceived;

    void DiscovererThread() {
      var beacon = new byte[128];
      EndPoint tracker_endpoint = new IPEndPoint(IPAddress.Any, 0);
      int length = discoverer_.ReceiveFrom(beacon, SocketFlags.None,
        ref tracker_endpoint);
    }
  }
}
