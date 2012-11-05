using System;
using System.Collections.Generic;
using System.Threading;
using Google.ProtocolBuffers;
using Nohros.Concurrent;
using R = Nohros.Resources;
using Nohros.Ruby.Protocol;

namespace Nohros.Ruby
{
  internal abstract partial class AbstractRubyMessageChannel :
    IRubyMessageChannel
  {
    const string kClassName = "Nohros.Ruby.AbstractRubyMessageChannel";

    readonly List<ListenerExecutorPair> listeners_;
    readonly IRubyLogger logger_;
    readonly Mailbox<RubyMessagePacket> mailbox_;
    bool channel_is_opened_;
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
    protected AbstractRubyMessageChannel() {
      mailbox_ = new Mailbox<RubyMessagePacket>(OnMessagePacket);
      listeners_ = new List<ListenerExecutorPair>();
      logger_ = RubyLogger.ForCurrentProcess;
      channel_is_opened_ = false;
    }
    #endregion

    /// <inheritdoc/>
    public virtual bool Send(IRubyMessage message) {
#if DEBUG
      if (!channel_is_opened_) {
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
        .SetId(ByteString.CopyFrom(message.Id))
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

    public abstract bool Send(RubyMessagePacket packet);

    /// <inheritdoc/>
    public virtual void Open() {
      if (!channel_is_opened_) {
        channel_is_opened_ = true;

        // create a dedicated thread to receive messages.
        receiver_thread_ = new Thread(ReceiveMessagePacket) {
          IsBackground = true
        };
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

    /// <inheritdoc/>
    public bool Send(byte[] message_id, int type, byte[] message,
      byte[] destination) {
      return Send(message_id, type, message, destination, string.Empty);
    }

    /// <inheritdoc/>
    public bool Send(byte[] message_id, int type, byte[] message,
      byte[] destination, string token) {
      RubyMessage request = new RubyMessage.Builder()
        .SetId(ByteString.CopyFrom(message_id))
        .SetMessage(ByteString.CopyFrom(message))
        .SetType(type)
        .SetSender(ByteString.CopyFrom(destination))
        .Build();
      return Send(request);
    }

    /// <summary>
    /// Get the next message packet from the channel, blocking the current
    /// </summary>
    /// <returns></returns>
    protected abstract RubyMessagePacket GetMessagePacket();

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
      while (channel_is_opened_) {
        mailbox_.Send(GetMessagePacket());
      }
    }

    /// <summary>
    /// Closes the communication channel.
    /// </summary>
    public void Close() {
      channel_is_opened_ = false;
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
          () => pair.Listener.OnMessagePacketReceived(packet));
      }
    }
  }
}
