using System;
using R = Nohros.Resources;
using Nohros.Ruby.Protocol;
using ZMQ;

namespace Nohros.Ruby
{
  /// <summary>
  /// The default implementation of the <see cref="IRubyMessageSender"/>
  /// interface.
  /// </summary>
  public class RubyMessageSender : AbstractRubyMessageSender, IRubyMessageSender,
                                   IDisposable
  {
    const string kClassName = "Nohros.Ruby.RubyMessageSender";

    readonly string endpoint_;
    readonly IRubyLogger logger_ = RubyLogger.ForCurrentProcess;
    readonly Socket socket_;
    bool is_channel_opened_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="RubyMessageSender"/> class
    /// using the specified context and sender endpoint.
    /// </summary>
    /// <param name="socket">
    /// A <see cref="Context"/> object that can be used to create instances
    /// of the <see cref="Socket"/> class.
    /// </param>
    public RubyMessageSender(Socket socket) {
      if (((int) socket.GetSockOpt(SocketOpt.TYPE)) != (int) SocketType.DEALER) {
        throw new ArgumentException(
          string.Format(R.StringResources.Arg_WrongType, "socket", "DEALER"));
      }

      logger_ = RubyLogger.ForCurrentProcess;
      socket_ = socket;
      is_channel_opened_ = false;
    }
    #endregion

    public void Dispose() {
      socket_.Dispose();
    }

    /// <summary>
    /// Opens the message sender's communication channel.
    /// </summary>
    /// <remarks>
    /// The channel should be opened before sending any message.
    /// </remarks>
    public void Open() {
      if (is_channel_opened_ == false) {
        is_channel_opened_ = true;
        socket_.Connect(endpoint_);
      }
    }

    /// <inheritdoc/>
    /// <remarks>
    /// The channel should be opened before sending any message. If the channel
    /// is not opened, <see cref="Send"/> will always returns <c>false</c>.
    /// </remarks>
    public override bool Send(RubyMessagePacket packet) {
      try {
        SendStatus status = socket_.SendMore();
        if (status == SendStatus.Sent) {
          status = socket_.Send(packet.ToByteArray());
          return status == SendStatus.Sent;
        }
      } catch (System.Exception exception) {
        logger_.Error(
          string.Format(R.StringResources.Log_MethodThrowsException, kClassName,
            "Send"), exception);
      }
      return false;
    }
  }
}
