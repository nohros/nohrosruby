using System;
using R = Nohros.Resources;
using Nohros.Ruby.Protocol;
using ZMQ;

namespace Nohros.Ruby
{
  internal class SelfMessageChannel : AbstractRubyMessageChannel,
                                      IRubyMessageChannel, IDisposable
  {
    const string kClassName = "Nohros.Ruby.SelfMessageChannel";

    readonly Context context_;
    readonly string message_channel_endpoint_;
    readonly Socket socket_;
    readonly IRubyLogger logger_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="SelfMessageChannel"/>
    /// class using the specified <see cref="Context"/> and channel
    /// endpoint address.
    /// </summary>
    /// <param name="context">
    /// A <see cref="Context"/> that can be used to create new sockets.
    /// </param>
    /// <param name="message_channel_endpoint">
    /// The address of the channel that is used to receive messages from the
    /// external world.
    /// </param>
    public SelfMessageChannel(Context context, string message_channel_endpoint) {
      socket_ = context.Socket(SocketType.ROUTER);
      message_channel_endpoint_ = message_channel_endpoint;
      logger_ = RubyLogger.ForCurrentProcess;
    }
    #endregion

    /// <inheritdoc/>
    public void Dispose() {
      socket_.Dispose();
    }

    /// <inheritdoc/>
    public override void Open() {
      // Open the socket before open the channel to ensure that the socket
      // is valid when GetMessagePacket is called.
      socket_.Connect(message_channel_endpoint_);
      base.Open();
    }

    /// <inheritdoc/>
    public override bool Send(RubyMessagePacket packet) {
      throw new NotImplementedException();
    }

    /// <inheritdoc/>
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
