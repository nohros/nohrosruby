using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Google.ProtocolBuffers;
using NUnit.Framework;
using Nohros.Concurrent;
using ZMQ;
using Telerik.JustMock;
using ZmqSocketType = ZMQ.SocketType;
using ZmqSocket = ZMQ.Socket;

namespace Nohros.Ruby
{
  public class SelfHostMessageChannelTests
  {
    Context context_;
    byte[] empty_byte_array_;
    IRubyMessageChannel forwarder_;
    Guid id_;
    IPEndPoint local_endpoint_;
    ZmqSocket router_;
    UdpClient udp_client_;

    [SetUp]
    public void SetUp() {
      context_ = new Context();
      router_ = context_.Socket(ZmqSocketType.ROUTER);
      router_.Bind("inproc://selfhostmessagechannel");
      forwarder_ = new RubyMessageChannel(context_,
        "inproc://selfhostmessagechannel");
      id_ = Guid.NewGuid();
      udp_client_ = new UdpClient(8530);
      local_endpoint_ = new IPEndPoint(IPAddress.Broadcast, 8530);
      empty_byte_array_ = new byte[0];
    }

    [Test]
    public void ShouldReceiveBeacons() {
      var signaler = new ManualResetEvent(false);
      var channel = new SelfHostMessageChannel(forwarder_, context_,
        "inproc://selfhost", udp_client_);
      channel.BeaconReceived += beacon => {
        Assert.That(beacon.PeerEndpoint.Port, Is.EqualTo(8520));
        Assert.That(beacon.PeerID, Is.EqualTo(id_));
        signaler.Set();
      };
      channel.Open();
      BeaconProto proto = new BeaconProto.Builder()
        .SetPeerId(ByteString.CopyFrom(id_.ToByteArray()))
        .SetPeerMailboxPort(8520)
        .Build();
      var bytes = new byte[proto.SerializedSize + 3];
      bytes[0] = (byte) 'R';
      bytes[1] = (byte) 'B';
      bytes[2] = (byte) 'Y';
      Buffer.BlockCopy(proto.ToByteArray(), 0, bytes, 3, proto.SerializedSize);
      udp_client_.Send(bytes, bytes.Length, local_endpoint_);

      if (!signaler.WaitOne(3000)) {
        Assert.Fail("Beacon was not received");
      }
      udp_client_.Close();
      channel.Dispose();
    }

    [Test]
    public void ShouldNotReceiveBeaconsWhenChannelIsClosed() {
      var signaler = new ManualResetEvent(false);
      var channel = new SelfHostMessageChannel(forwarder_, context_,
        "inproc://notreceivebeacons", udp_client_);
      channel.BeaconReceived += beacon => signaler.Set();
      BeaconProto proto = new BeaconProto.Builder()
        .SetPeerId(ByteString.CopyFrom(id_.ToByteArray()))
        .SetPeerMailboxPort(8520)
        .Build();
      var bytes = new byte[proto.SerializedSize + 3];
      bytes[0] = (byte) 'R';
      bytes[1] = (byte) 'B';
      bytes[2] = (byte) 'Y';
      Buffer.BlockCopy(proto.ToByteArray(), 0, bytes, 3, proto.SerializedSize);
      udp_client_.Send(bytes, bytes.Length, local_endpoint_);
      udp_client_.Close();

      if (signaler.WaitOne(3000)) {
        Assert.Fail("A beacon was received on a closed channel");
      }
    }

    [Test]
    public void ShouldReceiveSentMessages() {
      var signaler = new ManualResetEvent(false);
      var channel = new SelfHostMessageChannel(forwarder_, context_,
        "inproc://shouldreceivesentmessage", udp_client_);
      channel.AddListener(RubyMessageListeners.FromDelegate(packet => {
        Assert.That(packet.Header.Id.ToByteArray(),
          Is.EqualTo(id_.ToByteArray()));
        signaler.Set();
      }), Executors.SameThreadExecutor());
      channel.Open();

      var dealer = context_.Socket(ZmqSocketType.DEALER);
      dealer.Connect("inproc://shouldreceivesentmessage");

      var data = RubyMessages.CreateMessagePacket(id_.ToByteArray(), -1,
        empty_byte_array_);
      dealer.Send(data.ToByteArray());

      if (!signaler.WaitOne(3000)) {
        Assert.Fail("Message was not received.");
      }

      channel.Close();
      channel.Dispose();
    }
  }
}
