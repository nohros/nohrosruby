using System;
using Nohros.Resources;
using ZMQ;

namespace Nohros.Ruby
{
  /// <summary>
  /// The default implementation of the <see cref="IRubyMessageListener"/>
  /// interface
  /// </summary>
  internal class RubyMessageReceiver: IRubyMessageReceiver, IDisposable
  {
    const string kClassName = "Nohros.RubyRubyMessageReceiver";

    readonly Socket listener_;
    readonly IRubyLogger logger = RubyLogger.ForCurrentProcess;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="RubyMessageReceiver"/>
    /// class by using the specified listener socket.
    /// </summary>
    /// <param name="listener">
    /// A <see cref="Socket"/> that is used to listen for messages.
    /// </param>
    public RubyMessageReceiver(Socket listener) {
      listener_ = listener;
    }
    #endregion

    /// <inheritdoc/>
    public RubyMessagePacket GetMessagePacket() {
      try {
        byte[] message = listener_.Recv();
        if (message.Length > 0) {
          RubyMessagePacket packet = RubyMessagePacket.ParseFrom(message);
          return packet;
        }
      } catch (System.Exception exception) {
        logger.Error(string.Format(StringResources.Log_MethodThrowsException,
          kClassName, "GetMessagePacket"), exception);
      }
      return new RubyMessagePacket.Builder().SetSize(0).Build();
    }

    public void Dispose() {
      listener_.Dispose();
    }
  }
}
