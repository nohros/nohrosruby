using System;
using Nohros.Resources;
using Nohros.Ruby.Protocol;
using ZMQ;

namespace Nohros.Ruby
{
  /// <summary>
  /// The default implementation of the <see cref="IRubyMessageSender"/>
  /// interface.
  /// </summary>
  public class RubyMessageSender : IRubyMessageSender, IDisposable
  {
    const string kClassName = "Nohros.Ruby.RubyMessageSender";

    readonly IRubyLogger logger = RubyLogger.ForCurrentProcess;
    readonly Socket sender_;

    #region .ctor
    /// <param name="sender">
    /// A <see cref="Socket"/> that can be used to send messages to the ruby
    /// service node.
    /// </param>
    public RubyMessageSender(Socket sender) {
      sender_ = sender;
    }
    #endregion

    /// <inheritdoc/>
    public bool Send(IRubyMessage message) {
      try {
        SendStatus status = sender_.Send(message.ToByteArray());
        return status == SendStatus.Sent;
      } catch (System.Exception exception) {
        logger.Error(
          string.Format(StringResources.Log_MethodThrowsException, kClassName,
            "Send"), exception);
      }
      return false;
    }

    public void Dispose() {
      sender_.Dispose();
    }
  }
}
