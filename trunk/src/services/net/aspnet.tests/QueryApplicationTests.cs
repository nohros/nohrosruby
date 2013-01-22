using System;
using System.Collections.Generic;
using NUnit.Framework;
using Nohros;
using Nohros.Concurrent;
using Nohros.Ruby;
using Nohros.Ruby.Protocol;
using ZMQ;

namespace aspnet.tests
{
  [TestFixture]
  public class QueryApplicationTests
  {
    #region Setup/Teardown
    [SetUp]
    public void SetUp() {
      context_ = new Context();
      receiver_ = context_.Socket(SocketType.ROUTER);
      receiver_.Bind("inproc://127.0.0.1:6666");
      no_receiver_ = context_.Socket(SocketType.ROUTER);
      no_receiver_.Bind("inproc://127.0.0.1:6667");
      running_ = true;
      new BackgroundThreadFactory().CreateThread(Receive).Start();
    }
    #endregion

    Context context_;
    Socket receiver_, no_receiver_;
    bool running_;

    void Receive() {
      while (running_) {
        Queue<byte[]> data = receiver_.RecvAll();
        receiver_.SendMore(data.Dequeue());
        receiver_.SendMore(data.Dequeue());
        receiver_.Send(data.Dequeue());
      }
    }

    QueryRequest GetQueryRequest() {
      var bytes = new byte[] {0};
      return new QueryRequest(bytes, bytes, 0);
    }

    [Test]
    public void ShouldSendMessageToService() {
      var app = new QueryApplication(context_, "inproc://127.0.0.1:6666");
      app.Run(Executors.ThreadPoolExecutor());

      QueryRequest request = GetQueryRequest();
      IFuture<byte[]> response = null;
      response = app.ExecuteQuery(request);
      response.AsyncWaitHandle.WaitOne();
      Assert.That(response.IsCancelled, Is.EqualTo(false));
      Assert.That(response.IsCompleted, Is.EqualTo(true));
      byte[] data;
      Assert.That(response.TryGet(0, TimeUnit.Miliseconds, out data),
        Is.EqualTo(true));

      var packet = RubyMessagePacket.ParseFrom(data);
      Assert.That(packet.Message.Message, Is.EqualTo(new byte[] {0}));
    }

    [Test]
    public void ShouldTimeOut() {
      var app = new QueryApplication(context_, "inproc://127.0.0.1:6667");
      app.Run(Executors.ThreadPoolExecutor());

      QueryRequest request = GetQueryRequest();
      IFuture<byte[]> response = null;
      response = app.ExecuteQuery(request, 100);
      response.AsyncWaitHandle.WaitOne();
      Assert.That(response.IsCancelled, Is.EqualTo(false));
      Assert.That(response.IsCompleted, Is.EqualTo(true));
      byte[] data;
      try {
        response.TryGet(0, TimeUnit.Miliseconds, out data);
      } catch (ExecutionException e) {
        Assert.That(e.InnerException, Is.InstanceOf<TimeoutException>());
      }
      app.Stop();
    }
  }
}
