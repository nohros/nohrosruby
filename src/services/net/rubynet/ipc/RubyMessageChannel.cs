using System;
using System.Threading;
using Nohros.Concurrent;
using ZMQ;
using R = Nohros.Resources;
using Nohros.Ruby.Protocol;
using System.Collections.Generic;

namespace Nohros.Ruby
{
  /// <summary>
  /// A <see cref="RubyMessageChannel"/> handles the communication between
  /// external clients and the ruby service host.
  /// </summary>
  internal class RubyMessageChannel : AbstractRubyMessageSender,
                                      IRubyMessageChannel, IDisposable
  {
    class ListenerExecutorPair
    {
      readonly IExecutor executor_;
      readonly IRubyMessageListener listener_;

      #region .ctor
      /// <summary>
      /// Initializes a new instance of the <see cref="ListenerExecutorPair"/>
      /// class by using the specified listener and executor.
      /// </summary>
      /// <param name="listener">
      /// A <see cref="IRubyMessageListener"/> that should be called when a
      /// message is received by the channel.
      /// </param>
      /// <param name="executor">
      /// A <see cref="IExecutor"/> object that is used to execute the
      /// <see cref="IRubyMessageListener.OnMessagePacketReceived"/> callback.
      /// </param>
      /// <remarks>
      /// Each listener should be associated with a service(real or virtual).
      /// It will receive only the messages that is destinated to the
      /// associated service.
      /// </remarks>
      public ListenerExecutorPair(IRubyMessageListener listener,
        IExecutor executor) {
        listener_ = listener;
        executor_ = executor;
      }
      #endregion

      /// <summary>
      /// Gets the <see cref="IExecutor"/> object associated with the pair.
      /// </summary>
      public IExecutor Executor {
        get { return executor_; }
      }

      /// <summary>
      /// gets the <see cref="IRubyMessageListener"/> object associated with
      /// the pair.
      /// </summary>
      public IRubyMessageListener Listener {
        get { return listener_; }
      }
    }

    const string kClassName = "Nohros.Ruby.RubyMessageChannel";
    const string kSenderChannelEndpoint = "inproc://multiplex";

    static readonly byte[] true_byte_array_ = new byte[] {1};
    static readonly byte[] false_byte_array_ = new byte[] {0};

    readonly Context context_;
    readonly string endpoint_;
    readonly Socket internal_sender_socket_;
    readonly Socket ipc_socket_;
    readonly List<ListenerExecutorPair> listeners_;
    readonly IRubyLogger logger_;
    bool channel_is_opened_;
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
      ipc_socket_ = context_.Socket(SocketType.DEALER);
      internal_sender_socket_ = context.Socket(SocketType.REQ);
      listeners_ = new List<ListenerExecutorPair>();
      logger_ = RubyLogger.ForCurrentProcess;
      endpoint_ = endpoint;
    }
    #endregion

    public void Dispose() {
      internal_sender_socket_.Linger = 0;
      ipc_socket_.Linger = 0;
      internal_sender_socket_.Dispose();
      ipc_socket_.Dispose();
    }

    /// <inheritdoc/>
    public string Endpoint {
      get { return endpoint_; }
    }

    /// <inheritdoc/>
    public void Open() {
      if (!channel_is_opened_) {
        channel_is_opened_ = true;

        // Open the socket before open the channel to ensure that the socket
        // is valid when Multiplex is called.
        internal_sender_socket_.Bind(kSenderChannelEndpoint);
        ipc_socket_.Connect(endpoint_);

        // create a dedicated thread to receive messages.
        io_multiplexer_thread_ = new Thread(Multiplex) {
          IsBackground = true
        };
        io_multiplexer_thread_.Start();
      }
    }

    public override bool Send(RubyMessagePacket packet) {
      try {
        // Lets try to send the message using the NOBLOCK operation to avoid
        // checking if the channel is opened each time the Send method
        // is called.
        SendStatus status = internal_sender_socket_.Send(packet.ToByteArray(),
          SendRecvOpt.NOBLOCK);
        if (status == SendStatus.TryAgain) {
          // The NOBLOCK send operation fails, lets check if the channel is
          // opened and retry the send operation using the BLOCK method.
          if (!channel_is_opened_) {
            throw new InvalidOperationException(
              Resources.InvalidOperation_ClosedChannel);
          }
          status = internal_sender_socket_.Send(packet.ToByteArray());
          if (status == SendStatus.TryAgain) {
            return false;
          }
        }

        byte[] reply = internal_sender_socket_.Recv();
        return reply.Length > 0 && reply[0] == 1;
      } catch (System.Exception e) {
        logger_.Error(
          string.Format(R.StringResources.Log_MethodThrowsException, "Send",
            kClassName), e);
      }
      return false;
    }

    /// <inheritdoc/>
    public void AddListener(IRubyMessageListener listener, IExecutor executor) {
#if DEBUG
      if (listener == null || executor == null) {
        throw new ArgumentNullException(listener == null
          ? "listener"
          : "executor");
      }
#endif
      listeners_.Add(new ListenerExecutorPair(listener, executor));
    }

    /// <summary>
    /// Closes the communication channel.
    /// </summary>
    public void Close() {
      channel_is_opened_ = false;
    }

    /// <summary>
    /// The method that receives messages from the message receiver and
    /// dispatch them to the listener.
    /// </summary>
    /// <remarks>
    /// This method should runs in a dedicated thread.
    /// </remarks>
    void Multiplex() {
      var sender = context_.Socket(SocketType.REP);
      var items = new[] {
        sender.CreatePollItem(IOMultiPlex.POLLIN),
        ipc_socket_.CreatePollItem(IOMultiPlex.POLLIN)
      };
      items[0].PollInHandler += (socket, revents) => SendMessage(socket);
      items[1].PollInHandler += (socket, revents) => OnMessageReceived();

      sender.Connect(kSenderChannelEndpoint);
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

    void SendMessage(Socket socket) {
      try {
        // Get the message that should be sent over the DEALER socket from
        // the REP socket and reply with the status of the SEND operation.
        byte[] message = socket.Recv();
        SendStatus status = ipc_socket_.Send(message, SendRecvOpt.NOBLOCK);
        socket.Send((status == SendStatus.Sent)
          ? true_byte_array_
          : false_byte_array_);
      } catch (System.Exception e) {
        logger_.Error(
          string.Format(R.StringResources.Log_MethodThrowsException,
            "SendMessage", kClassName), e);
      }
    }

    /// <summary>
    /// The method that is called (by the mailbox) when a message is ready
    /// to be processed.
    /// </summary>
    void OnMessageReceived() {
      try {
        // Receive the message and dispatch it to the registered listeners.
        byte[] message = ipc_socket_.Recv(SendRecvOpt.NOBLOCK);
        RubyMessagePacket packet = RubyMessagePacket.ParseFrom(message);
        for (int i = 0, j = listeners_.Count; i < j; i++) {
          ListenerExecutorPair pair = listeners_[i];
          pair.Executor.Execute(
            () => pair.Listener.OnMessagePacketReceived(packet));
        }
      } catch (System.Exception e) {
        logger_.Error(
          string.Format(R.StringResources.Log_MethodThrowsException,
            "GetMessage",
            kClassName), e);
      }
    }
  }
}
