using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Google.ProtocolBuffers;
using Nohros.Concurrent;
using Nohros.Resources;
using Nohros.Ruby.Protocol;
using Nohros.Ruby.Protocol.Control;
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

    static readonly byte[] empty_frame_;
    readonly List<ListenerExecutorPair> listeners_;
    readonly IRubyLogger logger_;
    readonly Mailbox<RubyMessagePacket> mailbox_;
    readonly Socket socket_;
    bool is_opened_;
    Thread receiver_thread_;

    #region .ctor
    static RubyMessageChannel() {
      empty_frame_ = new byte[0];
    }

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
      // Send the message to the service for processing.
      RubyMessage response = message as RubyMessage ??
        RubyMessage.ParseFrom(message.ToByteArray());

      // Create the reply packed using the service processing result.
      int message_size = message.ToByteArray().Length;
      RubyMessageHeader header = new RubyMessageHeader.Builder()
        .SetId(message.Id)
        .SetSize(message_size)
        .Build();

      int header_size = header.SerializedSize;
      RubyMessagePacket packet = new RubyMessagePacket.Builder()
        .SetHeader(header)
        .SetHeaderSize(header.SerializedSize)
        .SetMessage(response)
        .SetSize(header_size + 2 + message_size)
        .Build();
      return Send(packet);
    }

    public bool Send(RubyMessagePacket packet) {
      try {
        // sent message should follow the pattern: [empty frame][message]
        SendStatus status = socket_.SendMore(empty_frame_);
        if (status == SendStatus.Sent) {
          socket_.Send(packet.ToByteArray());
          return status == SendStatus.Sent;
        }
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

        // create a dedicated thread to receive messages.
        receiver_thread_ = new Thread(ReceiveMessagePacket);
        receiver_thread_.IsBackground = true;
        receiver_thread_.Start();
      }
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
