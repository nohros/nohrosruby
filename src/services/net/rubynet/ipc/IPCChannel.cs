using System;
using System.Collections.Generic;
using System.Threading;
using Nohros.Concurrent;

namespace Nohros.Ruby
{
  /// <summary>
  /// A <see cref="IPCChannel"/> handles the communication between external
  /// clients and the ruby service host.
  /// </summary>
  internal partial class IPCChannel : IRubyMessageSender
  {
    readonly List<ListenerExecutorPair> listeners_;
    readonly Mailbox<RubyMessagePacket> mailbox_;
    readonly IRubyMessageReceiver receiver_;
    readonly Thread receiver_thread_;
    readonly IRubyMessageSender sender_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="IPCChannel"/> class by
    /// using the specified message's sender.
    /// </summary>
    /// <remarks>
    /// The <see cref="IPCChannel"/> object constructed through this
    /// constructor discards any received message.
    /// </remarks>
    public IPCChannel(IRubyMessageSender sender) {
#if DEBUG
      if (sender == null) {
        throw new ArgumentNullException("sender");
      }
#endif
      sender_ = sender;
      mailbox_ = new Mailbox<RubyMessagePacket>(OnMessagePacket);
      listeners_ = new List<ListenerExecutorPair>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IPCChannel"/> class by
    /// using the specified message's sender and receiver.
    /// </summary>
    public IPCChannel(IRubyMessageSender sender, IRubyMessageReceiver receiver)
      : this(sender) {
#if DEBUG
      if (receiver == null) {
        throw new ArgumentNullException("receiver");
      }
#endif
      receiver_ = receiver;

      // start the receiver thread as soon as possible to avoid missing
      // messages.
      receiver_thread_ = new Thread(ReceiveMessagePacket);
      receiver_thread_.IsBackground = true;
      receiver_thread_.Start();
    }
    #endregion

    /// <inheritdoc/>
    public bool Send(IRubyMessage message) {
      return sender_.Send(message);
    }

    /// <summary>
    /// The method that receives messages from the message receiver and
    /// dispatch them to the listener.
    /// </summary>
    /// <remarks>
    /// This method should runs in a dedicated thread.
    /// </remarks>
    void ReceiveMessagePacket() {
      mailbox_.Send(receiver_.GetMessagePacket());
    }

    /// <summary>
    /// The method that is called when a message is received.
    /// </summary>
    /// <param name="packet">
    /// The received message.
    /// </param>
    void OnMessagePacket(RubyMessagePacket packet) {
      for (int i = 0, j = listeners_.Count; i < j; i++) {
        ListenerExecutorPair pair = listeners_[i];
        string[] filters = pair.Listener.Filters;

        // If there are no filters, listener should receive all incoming
        // messages.
        if (filters.Length == 0) {
          OnMessagePacket(pair.Listener, pair.Executor, packet);
          continue;
        }

        // fail fast to avoid the loop.
        if (!packet.HasHeader) break;

        // Only the packets that match the filter should be delivered to the
        // listener.
        for (int k = 0, l = filters.Length; l < k; l++) {
          string filter = filters[k];
          if (
            string.Compare(filter, packet.Header.Service,
              StringComparison.OrdinalIgnoreCase) == 0) {
            OnMessagePacket(pair.Listener, pair.Executor, packet);
            break;
          }
        }
      }
    }

    void OnMessagePacket(IRubyMessageListener listener, IExecutor executor,
      RubyMessagePacket packet) {
      executor.Execute(delegate { listener.OnMessagePacketReceived(packet); });
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
      if (listener.Filters == null) {
        throw new ArgumentException("listeners.Filters is null");
      }
#endif
      // If listeners_ is null means that the IPC channel does not receive
      // messages, so just ignore the added listener since it will not be used.
      if (listeners_ != null) {
        listeners_.Add(new ListenerExecutorPair(listener, executor));
      }
    }
  }
}
