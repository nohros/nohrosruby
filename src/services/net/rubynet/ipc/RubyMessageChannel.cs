using System;
using R = Nohros.Resources;
using Nohros.Ruby.Protocol;
using ZMQ;

namespace Nohros.Ruby
{
  /// <summary>
  /// A <see cref="RubyMessageChannel"/> handles the communication between
  /// external clients and the ruby service host.
  /// </summary>
  internal class RubyMessageChannel : AbstractRubyMessageChannel,
                                      IRubyMessageChannel, IDisposable
  {
    const string kClassName = "Nohros.Ruby.RubyMessageChannel";

    readonly Context context_;
    readonly IRubyLogger logger_;
    readonly string message_channel_endpoint_;
    readonly Socket socket_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="RubyMessageChannel"/> class by
    /// using the specified message's sender.
    /// </summary>
    /// <remarks>
    /// The <see cref="RubyMessageChannel"/> object constructed through this
    /// constructor discards any received message.
    /// </remarks>
    public RubyMessageChannel(Context context, string message_channel_endpoint) {
#if DEBUG
      if (context == null || message_channel_endpoint == null) {
        throw new ArgumentNullException(context == null
          ? "socket"
          : "string message_channel_endpoint");
      }
#endif
      message_channel_endpoint_ = message_channel_endpoint;
      socket_ = context.Socket(SocketType.DEALER);
      logger_ = RubyLogger.ForCurrentProcess;
      context_ = context;
    }
    #endregion

    public void Dispose() {
      socket_.Dispose();
    }

    /// <inheritdoc/>
    public override void Open() {
      // Open the socket before open the channel to ensure that the socket
      // is valid when GetMessagePacket is called.
      socket_.Connect(Transport.TCP, message_channel_endpoint_);
      base.Open();
    }

    public override bool Send(RubyMessagePacket packet) {
      try {
        // sent message should follow the pattern: [empty frame][message]
        SendStatus status = socket_.SendMore();
        if (status == SendStatus.Sent) {
          socket_.Send(packet.ToByteArray());
          return status == SendStatus.Sent;
        }
      } catch (System.Exception exception) {
        logger_.Error(
          string.Format(R.StringResources.Log_MethodThrowsException, kClassName,
            "Send"), exception);
      }
      return false;
    }

    public override string Endpoint {
      get {
        int index = message_channel_endpoint_.IndexOf("://",
          StringComparison.Ordinal);
        return message_channel_endpoint_.Substring(index);
      }
    }

    protected override RubyMessagePacket GetMessagePacket() {
      try {
        byte[] message = socket_.Recv();
        if (message.Length > 0) {
          RubyMessagePacket packet = RubyMessagePacket.ParseFrom(message);
          return packet;
        }
      } catch (ZMQ.Exception exception) {
        if (exception.Errno == (int) ERRNOS.ETERM) {
          Close();
        }
      } catch (System.Exception exception) {
        logger_.Error(string.Format(R.StringResources.Log_MethodThrowsException,
          kClassName, "GetMessagePacket"), exception);
      }
      return new RubyMessagePacket.Builder().SetSize(0).Build();
    }
  }
}
