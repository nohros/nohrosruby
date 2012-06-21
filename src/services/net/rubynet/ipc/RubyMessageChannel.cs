using System;
using System.Collections.Generic;
using System.Threading;

using Nohros.Concurrent;
using Nohros.Resources;
using Nohros.Ruby.Protocol;
using ZMQ;

namespace Nohros.Ruby
{
  /// <summary>
  /// A <see cref="RubyMessageChannel"/> handles the communication between external
  /// clients and the ruby service host.
  /// </summary>
  internal partial class RubyMessageChannel : IRubyMessageChannel, IDisposable
  {
    const string kClassName = "Nohros.Ruby.RubyMessageChannel";

    readonly List<ListenerExecutorPair> listeners_;
    readonly IRubyLogger logger_;
    readonly Mailbox<RubyMessagePacket> mailbox_;
    readonly Socket socket_;
    bool is_opened_;
    Thread receiver_thread_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="RubyMessageChannel"/> class by
    /// using the specified message's sender.
    /// </summary>
    /// <remarks>
    /// The <see cref="RubyMessageChannel"/> object constructed through this
    /// constructor discards any received message.
    /// </remarks>
    public RubyMessageChannel(Socket socket) {
#if DEBUG
      if (socket == null) {
        throw new ArgumentNullException("socket");
      }
#endif
      socket_ = socket;
      mailbox_ = new Mailbox<RubyMessagePacket>(OnMessagePacket);
      listeners_ = new List<ListenerExecutorPair>();
      logger_ = RubyLogger.ForCurrentProcess;
      is_opened_ = false;
    }
    #endregion

    public void Dispose() {
      socket_.Dispose();
    }

    /// <inheritdoc/>
    public bool Send(IRubyMessage message) {
#if DEBUG
      if (!is_opened_) {
        logger_.Warn("Send() called on a closed channel.");
        return false;
      }
#endif

      try {
        SendStatus status = socket_.Send(message.ToByteArray());
        return status == SendStatus.Sent;
      } catch (System.Exception exception) {
        logger_.Error(
          string.Format(StringResources.Log_MethodThrowsException, kClassName,
            "Send"), exception);
      }
      return false;
    }

    /// <summary>
    /// Opens the communication channel.
    /// </summary>
    /// <remarks>
    /// A <see cref="RubyMessageChannel"/> should be opened to start receiving
    /// messages.
    /// </remarks>
    public void Open() {
      if (!is_opened_) {
        is_opened_ = true;

        // Tell the service node that we are ready.
        // TODO: (Send a handshake message).
        Send(null);

        // create a dedicated thread to receive messages.
        receiver_thread_ = new Thread(ReceiveMessagePacket);
        receiver_thread_.IsBackground = true;
        receiver_thread_.Start();
      }
    }

    /// <summary>
    /// Adds a listener to receive notifications for incoming messages.
    /// </summary>
    /// <param name="listener">
    /// A <see cref="IRubyMessageListener"/> that wants to receive
    /// notifications for incoming messages.
    /// </param>
    /// <param name="executor">
    /// A <see cref="IExecutor"/> that executes the
    /// <see cref="IRubyMessageListener.OnMessagePacketReceived"/> callback
    /// method.
    /// </param>
    public void AddListener(IRubyMessageListener listener, IExecutor executor) {
#if DEBUG
      if (listener == null || executor == null) {
        throw new ArgumentNullException(listener == null
          ? "listener"
          : "executor");
      }
#endif
      // If listeners_ is null means that the IPC channel does not receive
      // messages, so just ignore the added listener since it will not be used.
      if (listeners_ != null) {
        listeners_.Add(new ListenerExecutorPair(listener, executor));
      }
    }

    RubyMessagePacket GetMessagePacket() {
      try {
        byte[] message = socket_.Recv();
        if (message.Length > 0) {
          RubyMessagePacket packet = RubyMessagePacket.ParseFrom(message);
          return packet;
        }
      } catch (System.Exception exception) {
        logger_.Error(string.Format(StringResources.Log_MethodThrowsException,
          kClassName, "GetMessagePacket"), exception);
      }
      return new RubyMessagePacket.Builder().SetSize(0).Build();
    }

    /// <summary>
    /// The method that receives messages from the message receiver and
    /// dispatch them to the listener.
    /// </summary>
    /// <remarks>
    /// This method should runs in a dedicated thread.
    /// </remarks>
    void ReceiveMessagePacket() {
      // get the next message packet and store it in mailbox for futher
      // processing.
      mailbox_.Send(GetMessagePacket());
    }

    /// <summary>
    /// The method that is called (by the mailbox) when a message is ready
    /// to be processed.
    /// </summary>
    /// <param name="packet">
    /// The received message packet.
    /// </param>
    void OnMessagePacket(RubyMessagePacket packet) {
      for (int i = 0, j = listeners_.Count; i < j; i++) {
        ListenerExecutorPair pair = listeners_[i];
        pair.Executor.Execute(
          delegate { pair.Listener.OnMessagePacketReceived(packet); });
      }
    }
  }
}
