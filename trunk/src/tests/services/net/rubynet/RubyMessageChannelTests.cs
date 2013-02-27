using System;
using System.Threading;
using NUnit.Framework;
using Nohros.Concurrent;
using Nohros.Ruby.Extensions;
using Nohros.Ruby.Protocol;
using ZMQ;

namespace Nohros.Ruby
{
  [TestFixture]
  public class RubyMessageChannelTests
  {
    #region Setup/Teardown
    [SetUp]
    public void SetUp() {
      context_ = new Context();
    }
    #endregion

    Context context_;

    [Test]
    public void ShouldNotSendMessageWhenChannelIsClosed() {
      using (
        var channel = new RubyMessageChannel(context_, "inproc://channel")) {
        var bytes = "message".AsBytes();
        Assert.That(() => channel.Send(bytes, 10, bytes, bytes),
          Throws.InvalidOperationException);
      }
    }

    [Test]
    public void ShouldReceiveSentMessage() {
      var signaler = new ManualResetEvent(false);
      var socket = context_.Socket(SocketType.DEALER);
      socket.Bind("inproc://channel");
      using (var channel = new RubyMessageChannel(context_, "inproc://channel")) {
        channel.Open();
        channel.AddListener(RubyMessageListeners.FromDelegate(packet => {
          Assert.That(packet.Message.Message.ToByteArray().AsString(),
            Is.EqualTo("message"));
          signaler.Set();
        }), Executors.SameThreadExecutor());
        signaler.WaitOne(3000);
      }
    }

    [Test]
    public void ShouldSendMessage() {
      var socket = context_.Socket(SocketType.ROUTER);
      socket.Bind("inproc://shouldSendMessage");
      var bytes = "message".AsBytes();
      using (var channel = new RubyMessageChannel(context_,
        "inproc://shouldSendMessage")) {
        channel.Open();
        ThreadPool.QueueUserWorkItem(
          obj => channel.Send(bytes, 10, bytes, bytes));

        try {
          byte[] address = socket.Recv(3000);
          byte[] message = socket.Recv(3000);
          Assert.That(message, Is.Not.Null);

          RubyMessagePacket packet = RubyMessagePacket.ParseFrom(message);
          Assert.That(packet.Message.Message.ToByteArray().AsString(),
            Is.EqualTo("message"));
        } catch (System.Exception e) {
          Assert.Fail(e.Message);
        }
      }
    }
  }
}
