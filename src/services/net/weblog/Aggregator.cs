using System;

using System.Net.Sockets;
using System.Text;
using Nohros.Concurrent;
using Nohros.Data.Json;

namespace Nohros.Ruby.Weblog
{
  /// <summary>
  /// A class that aggregates log messages published by a
  /// <see cref="IRubyLogger"/> and outputs then throught a TCP socket.
  /// </summary>
  public class Aggregator
  {
    Socket publisher_;
    ZMQ.Socket subscriber_;

    IWeblogLogger logger = WeblogLogger.ForCurrentProcess;

    /// <summary>
    /// Initializes a new instance of the <see cref="Aggregator"/> class
    /// by using the specified TCP and ZMQ sockets.
    /// </summary>
    /// <param name="publisher">
    /// A <see cref="Socket"/> that can be used to send tcp packets.
    /// </param>
    /// <param name="subscriber">
    /// A <see cref="ZMQ.Socket "/> that can be used to subscribe to a zeromq
    /// publisher socket.
    /// </param>
    public Aggregator(Socket publisher, ZMQ.Socket subscriber) {
      publisher_ = publisher;
      subscriber_ = subscriber;
    }

    /// <summary>
    /// Subscribes to the zeromq publisher located at <see cref="address"/>.
    /// </summary>
    /// <param name="host">
    /// The name of the remote publisher host.
    /// </param>
    /// <param name="port">
    /// The port number of the remote publisher host.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="host"/> is null.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The port number is not valid.
    /// </exception>
    /// <remarks>
    /// </remarks>
    public bool Subscribe(string host, int port) {
      try {
        string address = "tcp://" + host + ":" + port.ToString();

        #region : logging :
        if (logger.IsDebugEnabled) {
          logger.Debug("Subscribing to publisher located at:" + address);
        }
        #endregion

        subscriber_.Connect(address);
        return true;
      } catch(ZMQ.Exception exception) {
        #region : logging :
        logger.Error("Subscription failed", exception);
        #endregion
      }
      return false;
    }

    public void Run() {
      try {
        byte[] data = subscriber_.Recv();
        LogMessage message = LogMessage.ParseFrom(data);

        JsonStringBuilder builder = new JsonStringBuilder()
          .WriteBeginObject()
          .WriteMember("level", message.Level)
          .WriteMember("message", message.Message)
          .WriteMember("", message.TimeStamp)
          .WriteMember("", message.Exception);

        byte[] log_message_byte_array = Encoding.UTF8.GetBytes(builder.ToString());
        publisher_.Send(log_message_byte_array);
      } catch(ZMQ.Exception zmq_exception) {
        #region : logging :
        logger.Error("ZMQ exception", zmq_exception);
        #endregion
      } catch(SocketException socket_exception) {
        #region : logging :
        logger.Error("Socket exception", socket_exception);
        #endregion
      } catch(Exception exception) {
        #region : logging :
        logger.Error("", exception);
        #endregion
      }
    }
  }
}
