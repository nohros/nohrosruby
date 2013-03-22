using System;
using System.Threading;
using System.Collections.Generic;
using Nohros.Concurrent;
using R = Nohros.Resources;
using Nohros.Ruby.Protocol;
using ZMQ;
using ZmqSocket = ZMQ.Socket;

namespace Nohros.Ruby
{
  /// <summary>
  /// A <see cref="RubyMessageChannel"/> handles the communication between
  /// external clients and the ruby service host.
  /// </summary>
  internal class RubyMessageChannel : MultiplexedMessageChannel,
                                      IRubyMessageChannel, IDisposable
  {
    const string kClassName = "Nohros.Ruby.RubyMessageChannel";
    const string kSenderChannelEndpoint = "inproc://ruby-multiplex-channel";

    readonly Context context_;
    readonly string endpoint_;
    readonly List<ListenerExecutorPair> listeners_;
    readonly IRubyLogger logger_;
    volatile bool channel_is_opened_;
    Thread io_multiplexer_thread_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="RubyMessageChannel"/>
    /// class by using the specified message's receiver and sender endpoints.
    /// </summary>
    /// 
    public RubyMessageChannel(Context context, string endpoint) {
#if DEBUG
      if (context == null || endpoint == null) {
        throw new ArgumentNullException(context == null
          ? "context"
          : "endpoint");
      }
#endif
      context_ = context;
      listeners_ = new List<ListenerExecutorPair>();
      logger_ = RubyLogger.ForCurrentProcess;
      endpoint_ = endpoint;
    }
    #endregion

    /// <inheritdoc/>
    public void Dispose() {
      Close(0);
    }

    /// <inheritdoc/>
    public void Close() {
      Close(0);
    }

    /// <inheritdoc/>
    public void Close(int timeout) {
      // Closes the channel only if it is open.
      if (channel_is_opened_) {
        channel_is_opened_ = false;

        // Wait the multiplex channel to finish.
        io_multiplexer_thread_.Join(timeout);
      }
    }

    /// <inheritdoc/>
    public void Open() {
      if (!channel_is_opened_) {
        channel_is_opened_ = true;

        // Open the socket before open the channel to ensure that the socket
        // is valid when Multiplex is called.

        // create a dedicated thread to receive messages.
        io_multiplexer_thread_ = new Thread(Multiplex) {
          IsBackground = true
        };
        io_multiplexer_thread_.Start();
      }
    }

    /// <inheritdoc/>
    public override bool Send(RubyMessagePacket packet) {
      using (ZmqSocket socket = context_.Socket(SocketType.REQ)) {
        try {
          socket.Connect(kSenderChannelEndpoint);

          // Lets try to send the message using the NOBLOCK operation to avoid
          // checking if the channel is opened each time the Send method
          // is called.
          SendStatus status = socket.Send(packet.ToByteArray(),
            SendRecvOpt.NOBLOCK);
          if (status == SendStatus.TryAgain) {
            // The NOBLOCK send operation fails, lets check if the channel is
            // opened and retry the send operation using the BLOCK method.
            if (!channel_is_opened_) {
              throw new InvalidOperationException(
                Resources.InvalidOperation_ClosedChannel);
            }
            status = socket.Send(packet.ToByteArray());
            if (status == SendStatus.TryAgain) {
              return false;
            }
          }

          byte[] reply = socket.Recv();
          return reply.Length > 0 && reply[0] == 1;
        } catch (ZMQ.Exception e) {
          logger_.Error(
            string.Format(R.StringResources.Log_MethodThrowsException, "Send",
              kClassName), e);
        }
        return false;
      }
    }

    /// <inheritdoc/>
    public void AddListener(IRubyMessagePacketListener packet_listener, IExecutor executor) {
#if DEBUG
      if (listener == null || executor == null) {
        throw new ArgumentNullException(listener == null
          ? "listener"
          : "executor");
      }
#endif
      listeners_.Add(new ListenerExecutorPair(packet_listener, executor));
    }

    /// <summary>
    /// The method that receives messages from the message receiver and
    /// dispatch them to the listener.
    /// </summary>
    /// <remarks>
    /// This method should runs in a dedicated thread.
    /// </remarks>
    void Multiplex() {
      var inproc_socket = context_.Socket(SocketType.REP);
      inproc_socket.Bind(kSenderChannelEndpoint);

      var ipc_socket = context_.Socket(SocketType.DEALER);
      ipc_socket.Connect(endpoint_);

      var items = new[] {
        inproc_socket.CreatePollItem(IOMultiPlex.POLLIN),
        ipc_socket.CreatePollItem(IOMultiPlex.POLLIN)
      };
      items[0].PollInHandler += (socket, revents) => Proxy(socket, ipc_socket);
      items[1].PollInHandler += (socket, revents) => OnMessageReceived(socket);

      while (channel_is_opened_) {
        try {
          context_.Poll(items);
        } catch (ZMQ.Exception e) {
          // Prevent the normal context termination to raise an exception.
          if (e.Errno == (int) ERRNOS.ETERM) {
            return;
          }
          throw;
        }
      }
    }

    /// <summary>
    /// The method that is called (by the mailbox) when a message is ready
    /// to be processed.
    /// </summary>
    void OnMessageReceived(Socket socket) {
      try {
        // Receive the message and dispatch it to the registered listeners.
        byte[] message = socket.Recv();
        if (message.Length > 0) {
          RubyMessagePacket packet = RubyMessagePacket.ParseFrom(message);
          for (int i = 0, j = listeners_.Count; i < j; i++) {
            ListenerExecutorPair pair = listeners_[i];
            pair.Executor.Execute(
              () => pair.Listener.OnMessagePacketReceived(packet));
          }
        }
      } catch (System.Exception e) {
        logger_.Error(
          string.Format(R.StringResources.Log_MethodThrowsException,
            "GetMessage", kClassName), e);
      }
    }
  }
}
