using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZMQ;
using ZmqSocket = ZMQ.Socket;
using Exception = System.Exception;
using R = Nohros.Resources.StringResources;

namespace Nohros.Ruby
{
  internal abstract class MultiplexedMessageChannel : AbstractRubyMessageSender
  {
    const string kClassName = "Nohros.Ruby.AbstractRubyMessageSender";

    static readonly byte[] true_byte_array_ = new byte[] {1};
    static readonly byte[] false_byte_array_ = new byte[] {0};

    readonly RubyLogger logger_;

    #region .ctor
    protected MultiplexedMessageChannel() {
      logger_ = RubyLogger.ForCurrentProcess;
    }
    #endregion

    /// <summary>
    /// Gets a message from the <seealso cref="inproc_socket"/> socket and send
    /// it through the <seealso cref="ipc_socket"/> socket.
    /// </summary>
    /// <param name="inproc_socket">
    /// The <seealso cref="ZmqSocket"/> objetc where the messages should be get.
    /// </param>
    /// <param name="ipc_socket">
    /// The <see cref="ZmqSocket"/> object where the received messages should
    /// be proxied.
    /// </param>
    protected void Proxy(ZmqSocket inproc_socket, ZmqSocket ipc_socket) {
      byte[] response = false_byte_array_;
      try {
        // Get the message that should be sent over the DEALER socket from
        // the REP socket and reply with the status of the SEND operation.
        Queue<byte[]> parts = inproc_socket.RecvAll();
#if DEBUG
        if (parts.Count != 2) {
          throw new ArgumentException("Received a invalid number of parts.");
        }
#endif
        ipc_socket.SendMore(parts.Dequeue());
        SendStatus status = ipc_socket.Send(parts.Dequeue());
        if (status == SendStatus.Sent) {
          response = true_byte_array_;
        }
      } catch (Exception e) {
        logger_.Error(string.Format(R.Log_MethodThrowsException, "SendMessage",
          kClassName), e);
        response = false_byte_array_;
      }
      try {
        inproc_socket.Send(response);
      } catch {
        // there is nothing we can do if the send operation over the inproc
        // socket fails.
      }
    }
  }
}
