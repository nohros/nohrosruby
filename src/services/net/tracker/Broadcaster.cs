using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Google.ProtocolBuffers;
using Nohros.Concurrent;
using Nohros.Ruby.Extensions;
using R = Nohros.Resources.StringResources;

namespace Nohros.Ruby
{
  public class Broadcaster
  {
    const string kClassName = "Nohros.Ruby.Broadcaster";

    readonly RubyLogger logger_;
    readonly ManualResetEvent start_stop_event_;
    readonly UdpClient udp_client_;
    volatile int broadcast_interval_;
    volatile int broadcast_port_;
    volatile byte[] peer_id_;
    volatile bool running_;

    #region .ctor
    /// <summary>
    /// Intializes a new instance of the <see cref="Broadcaster"/> class using
    /// te specified udp client and thread factory.
    /// </summary>
    /// <param name="udp_client">
    /// A <see cref="UdpClient"/> that can be used to send broadcast messages.
    /// </param>
    public Broadcaster(UdpClient udp_client) {
      udp_client_ = udp_client;
      start_stop_event_ = new ManualResetEvent(false);
      broadcast_interval_ = 30;
      broadcast_port_ = 8520;
      logger_ = RubyLogger.ForCurrentProcess;
      peer_id_ = null;
      running_ = false;
    }
    #endregion

    public void Listen() {
      running_ = true;
      udp_client_.BeginReceive(ReceiveBeacon, null);
    }

    /// <summary>
    /// Raised when a beacon is received from a tracker.
    /// </summary>
    public event BeaconReceivedEventHandler BeaconReceived;

    void OnBeaconReceived(Beacon beacon) {
      Listeners.SafeInvoke<BeaconReceivedEventHandler>(BeaconReceived,
        handler => handler(beacon));
    }

    void ReceiveBeacon(IAsyncResult result) {
      try {
        var endpoint = new IPEndPoint(IPAddress.Any, 0);
        var bytes = udp_client_.EndReceive(result, ref endpoint);
        if (bytes.Length > 0) {
          var beacon = Beacon.FromByteArray(bytes, endpoint.Address);
          OnBeaconReceived(beacon);
          if (running_) {
            udp_client_.BeginReceive(ReceiveBeacon, null);
          }
        }
      } catch (SocketException e) {
        // If the socket has been shutdown finish the thread.
        if (e.SocketErrorCode == SocketError.Shutdown) {
          return;
        }
        logger_.Error(string.Format(
          R.Log_MethodThrowsException, "Broadcast", kClassName), e);
      } catch (FormatException e) {
        logger_.Error(string.Format(R.Log_MethodThrowsException,
          "Broadcast", kClassName), e);
      }
    }

    /// <summary>
    /// Start sending broadcast messages at a regular interval.
    /// </summary>
    /// <remarks>
    /// The broadcast messages is
    /// </remarks>
    public void Broadcast(byte[] peer_id, int mailbox) {
      start_stop_event_.Reset();

      peer_id_ = peer_id;

      byte[] broadcast = new BeaconMessage.Builder()
        .SetPeerId(ByteString.CopyFrom(peer_id))
        .SetPeerMailboxPort(mailbox)
        .Build()
        .ToByteArray();

      byte[] beacon = new byte[broadcast.Length + 3];
      beacon[0] = (byte) 'R';
      beacon[1] = (byte) 'B';
      beacon[2] = (byte) 'Y';
      Array.Copy(broadcast, 0, beacon, 3, broadcast.Length);

      var endpoint = new IPEndPoint(IPAddress.Broadcast, broadcast_port_);
      udp_client_.BeginSend(beacon, beacon.Length, endpoint, Broadcast,
        beacon);
    }

    void Broadcast(IAsyncResult result) {
      var broadcast = (byte[]) result.AsyncState;
      try {
        int sent = udp_client_.EndSend(result);
        if (sent != broadcast.Length) {
          logger_.Warn("Number of sent bytes is lesser than broadcast size.");
        }

        // TODO: Create a way to stop the broadcasting.
        if (!start_stop_event_.WaitOne(TimeSpan.FromSeconds(broadcast_interval_))) {
          var endpoint = new IPEndPoint(IPAddress.Broadcast, broadcast_port_);
          udp_client_.BeginSend(broadcast, broadcast.Length, endpoint, Broadcast,
            broadcast);
        }
      } catch (SocketException e) {
        logger_.Error(string.Format(R.Log_MethodThrowsException, "Broadcast",
          kClassName), e);
      }
    }

    public void Stop() {
      start_stop_event_.Set();
      running_ = false;
    }

    /// <summary>
    /// Gets or sets the interval, in seconds, between two broadcast messages.
    /// </summary>
    /// <value>The interval, in seconds, between two broadcast messages.</value>
    public int BroadcastInterval {
      get { return broadcast_interval_; }
      set { broadcast_interval_ = value; }
    }

    public int BroadcastPort {
      get { return broadcast_port_; }
      set { broadcast_port_ = value; }
    }

    internal byte[] PeerID {
      get { return peer_id_; }
    }
  }
}
