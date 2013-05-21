using System;
using System.Collections.Generic;
using System.Threading;
using Google.ProtocolBuffers;
using Nohros.Concurrent;
using Nohros.Resources;
using ZMQ;
using ZmqContext = ZMQ.Context;
using ZmqSocket = ZMQ.Socket;

namespace Nohros.Ruby.Logging
{
  public class MessageChannel
  {
    const string kClassName = "Nohros.Ruby.Logging.MessageChannel";

    readonly ZmqContext context_;
    readonly string endpoint_;
    readonly RubyLogLogger logger_;
    volatile bool channel_is_opened_;
    Thread subscriber_thread_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="MessageChannel"/> using
    /// the given <see cref="ZmqContext"/>
    /// </summary>
    public MessageChannel(ZmqContext context, string endpoint) {
      context_ = context;
      endpoint_ = endpoint;
      logger_ = RubyLogLogger.ForCurrentProcess;
    }
    #endregion

    public void Open(IThreadFactory thread_factory) {
      if (!channel_is_opened_) {
        channel_is_opened_ = true;

        // Open the socket before open the channel to ensure that the socket
        // is valid when Multiplex is called.

        // create a dedicated thread to receive messages.
        subscriber_thread_ = thread_factory.CreateThread(ThreadMain);
        subscriber_thread_.Start();
      }
    }

    void ThreadMain() {
      ZmqSocket socket = context_.Socket(SocketType.SUB);
      socket.Connect(endpoint_);
      socket.Subscribe(new byte[0]);
      while (channel_is_opened_) {
        try {
          Queue<byte[]> parts = socket.RecvAll();
          if (parts.Count == 2) {
            parts.Dequeue(); // discard the subscription.
            byte[] message = parts.Dequeue();
            LogMessage log = LogMessage.ParseFrom(ByteString.CopyFrom(message));
            OnMessageReceived(log);
          }
        } catch (ZMQ.Exception ze) {
          // TODO: Check for context disposition.
        } catch (System.Exception e) {
          logger_.Error(string.Format(StringResources.Log_MethodThrowsException,
            "ThreadMain", kClassName), e);
        }
      }
    }

    public event MessageReceivedEventHandler MessageReceived;

    void OnMessageReceived(LogMessage message) {
      Listeners
        .SafeInvoke<MessageReceivedEventHandler>(MessageReceived,
          handler => handler(message));
    }

    public void Close(int timeout = Timeout.Infinite) {
      channel_is_opened_ = false;
      subscriber_thread_.Join();
    }
  }
}
