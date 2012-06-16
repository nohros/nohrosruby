using System;
using Nohros.Concurrent;

namespace Nohros.Ruby
{
  /// <summary>
  /// A <see cref="IPCChannel"/> handles the communication between external
  /// clients and the ruby service host.
  /// </summary>
  internal class IPCChannel : IRubyMessageSender
  {
    readonly IRubyMessageSender sender_;
    readonly IRubyMessageReceiver receiver_;
    readonly ExecutionList execution_list_;

    RubyMessageHandlerDelegate message_packet_received_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="IPCChannel"/> class by
    /// using the specified message's sender.
    /// </summary>
    /// <remarks>
    /// The <see cref="IPCChannel"/> object constructed through this
    /// constructor does not receive messages.
    /// </remarks>
    public IPCChannel(IRubyMessageSender sender) {
#if DEBUG
      if (sender == null) {
        throw new ArgumentNullException("sender");
      }
#endif
      sender_ = sender;
      execution_list_ = null;
      receiver_ = null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IPCChannel"/> class by
    /// using the specified message's sender and receiver.
    /// </summary>
    public IPCChannel(IRubyMessageSender sender, IRubyMessageReceiver receiver) {
      sender_ = sender;
      receiver_ = receiver;
      execution_list_ = new ExecutionList();
    }
    #endregion

    /// <inheritdoc/>
    public bool Send(IRubyMessage message) {
      return sender_.Send(message);
    }

    public void AddListener(IRubyMessageListener listener, IExecutor executor) {
      // If execution_list_ is null means that the IPC channel does not receive
      // messages, so just ignore the added listener since we eill never use it.
      if (execution_list_ != null) {
        execution_list_.Add(delegate {
          listener.OnMessagePacketReceived(packet);
        }, executor);
      }
    }
  }
}
