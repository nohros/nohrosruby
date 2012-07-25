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
  internal class RubyMessageReceiver: IRubyMessageReceiver, IDisposable
  {
    const string kClassName = "Nohros.RubyRubyMessageReceiver";

    readonly Socket receiver_;
    readonly IRubyLogger logger = RubyLogger.ForCurrentProcess;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="RubyMessageReceiver"/>
    /// class by using the specified listener socket.
    /// </summary>
    /// <param name="receiver">
    /// A <see cref="Socket"/> that is used to listen for messages.
    /// </param>
    public RubyMessageReceiver(Socket receiver) {
      receiver_ = receiver;
    }
    #endregion

    /// <inheritdoc/>
    public RubyMessagePacket GetMessagePacket() {
      try {
        byte[] message = receiver_.Recv();
        if (message.Length > 0) {
          RubyMessagePacket packet = RubyMessagePacket.ParseFrom(message);
          return packet;
        }
      } catch (System.Exception exception) {
        logger.Error(string.Format(R.StringResources.Log_MethodThrowsException,
          kClassName, "GetMessagePacket"), exception);
      }
      return new RubyMessagePacket.Builder().SetSize(0).Build();
    }

    public void Dispose() {
      receiver_.Dispose();
    }
  }
}
