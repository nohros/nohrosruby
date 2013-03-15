using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using ZMQ;

namespace Nohros.Ruby
{
  public class TrackerMessageChannelTests
  {
    Context context_;
    byte[] empty_byte_array_;

    [SetUp]
    public void SetUp() {
      context_ = new Context();
      empty_byte_array_ = new byte[0];
    }

    [Test]
    public void ShouldNotSendMessagesWhenChannelIsClosed() {
      var channel = new TrackerMessageChannel(context_,
        (ZMQEndpoint) "inproc://shouldNotSendMessageWhenChannelIsClosed");
      TestDelegate method =
        () => channel.Send(empty_byte_array_, 0, empty_byte_array_,
          empty_byte_array_);
      Assert.That(method, Throws.InvalidOperationException);
    }
  }
}
