using System;
using NUnit.Framework;
using Nohros.Concurrent;
using Nohros.Ruby;
using Telerik.JustMock;
using ZMQ;

namespace aspnet.tests
{
  public class QueryApplicationTests
  {
    [Test]
    public void ShouldSendMessageToService() {
      var socket = Mock.Create<Socket>();
      Mock
        .Arrange(() => socket.Send())
        .Returns(SendStatus.Sent);
      Mock
        .Arrange(() => socket.Connect(Arg.IsAny<string>()))
        .DoInstead(() => { });

      var context = Mock.Create<Context>();
      Mock
        .Arrange(() => context.Socket(Arg.IsAny<SocketType>()))
        .Returns(socket);

      var app = new QueryApplication(context, "127.0.0.1",
        new BackgroundThreadFactory());
      app.Start();
    }
  }
}
