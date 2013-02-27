using System;
using System.Text;
using Nohros.Resources;
using Nohros.Ruby.Protocol;
using ZMQ;
using R = Nohros.Resources;

namespace Nohros.Ruby
{
  /// <summary>
  /// The default implementation of the <see cref="IRubyMessageListener"/>
  /// interface
  /// </summary>
  internal class RubyMessageReceiver : IRubyMessageReceiver
  {
    const string kClassName = "Nohros.Ruby.RubyMessageReceiver";

    readonly string endpoint_;
    readonly IRubyLogger logger_ = RubyLogger.ForCurrentProcess;
    readonly Socket socket_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="RubyMessageReceiver"/>
    /// class by using the specified receiver context and endpoint.
    /// </summary>
    public RubyMessageReceiver(Socket socket) {
      socket_ = socket;
    }
    #endregion

    /// <inheritdoc/>
    public RubyMessagePacket GetMessagePacket() {
      try {
        byte[] message = socket_.Recv();
        if (message.Length > 0) {
          RubyMessagePacket packet = RubyMessagePacket.ParseFrom(message);
          return packet;
        }
      } catch (ZMQ.Exception exception) {
        if (exception.Errno == (int) ERRNOS.ETERM) {
          // TODO: close the comunication channel.
        }
      } catch (System.Exception exception) {
        logger_.Error(string.Format(R.StringResources.Log_MethodThrowsException,
          kClassName, "CreateMessagePacket"), exception);
      }
      return new RubyMessagePacket.Builder().SetSize(0).Build();
    }
  }
}
