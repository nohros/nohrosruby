using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ZMQ;

using Google.ProtocolBuffers;

namespace Nohros.Ruby.Service.Net
{
  /// <summary>
  /// .NET implementation of the <see cref="IRubyServiceHost"/> interface. This
  /// class is used to host a .NET based ruby services.
  /// </summary>
  internal class RubyServiceHost : IRubyServiceHost
  {
    const int kTimeout = 30000;

    readonly IRubyService service_;
    readonly Socket socket_;

    #region .ctor
    public RubyServiceHost(IRubyService service) {
      service_ = service;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RubyServiceHost"/> by
    /// using the specified service and zeromq reply socket.
    /// </summary>
    /// <param name="service">
    /// The service to be hosted by this service host.
    /// </param>
    /// <param name="socket">
    /// A zeromq socket whose type is REP, and is used to handle the
    /// communication with the service.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="service"/> or <paramref name="socket"/> is
    /// <c>null</c>
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The type of <paramref name="socket"/> is not
    /// <see cref="SocketType.REP"/>.
    /// </exception>
    public RubyServiceHost(IRubyService service, Socket socket) {
      if (service == null || socket == null) {
        throw new ArgumentNullException(socket == null ? "socket" : "service");
      }

      if (!socket.GetSockOpt(SocketOpt.TYPE).Equals(SocketType.REP)) {
        throw new ArgumentOutOfRangeException(
          Resources.log_zmq_socket_is_not_of_type);
      }

      service_ = service;
      socket_ = socket;
    }
    #endregion

    /// <summary>
    /// Starts the hosted service.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The hosted service runs into a dedicated thread. The thread where
    /// this code is running is used to send/receive messages to/from the
    /// service.
    /// </para>
    /// <para>
    /// This method does not return until the running hosted service have
    /// finished your execution.
    /// </para>
    /// <para>
    /// If the service throws any exception this is propaggated to the
    /// caller and the service is forced to stop.
    /// </para>
    /// </remarks>
    public void StartService() {
      service_.Start();
    }

    /// <summary>
    /// Stops the hosted service.
    /// </summary>
    /// <remarks>
    /// </remarks>
    public void StopService() {
      service_.Stop();
    }

    /// <summary>
    /// Handle service messages sent from the Ruby Service Host.
    /// </summary>
    /// <remarks>
    /// The ruby agent communicates with the service host through a named pipe.
    /// Each service host must create a pipe with the name that was specified
    /// by the ruby agent service and wait for the connection of the agent.
    /// <para>
    /// This method receives a message from the ruby server, parse it, packet
    /// it into a new <see cref="IRubyService"/> object and send it to the
    /// hosted service.
    /// </para>
    /// </remarks>
    void MessageHandler(IAsyncResult result) {
      AsyncPipeState async_state = result.AsyncState as AsyncPipeState;
      NamedPipeClientStream pipe_stream = async_state.PipeStream;

      IRubyLogger logger = RubyLogger.ForCurrentProcess;

      // ends the asynchronous operation and get the results.
      int readed = 0;
      try {
        readed = pipe_stream_.EndRead(result);
      } catch (IOException io_exception) {
        logger.Error("[MessageHandler   Nohros.Ruby.Service.Net.RubyNet]   " +
            ((pipe_stream.IsConnected) ? "The stream is closed" : "an internal error has occured while reading data from the server."), io_exception);
      } catch (Exception exception) {
        // handle exceptions that has occurred during the asynchronous read
        // operation.
        logger.Error("[MessageHandler   Nohros.Ruby.Service.Net.RubyNet]", exception);
      }

      // the pipe has been disconnected
      if (readed == 0) return;

      // convert the readed data to an instance of a IRubyMessagePacket and
      // pass them to the service.
      try {
        RubyMessagePacket message_packet =
          RubyMessagePacket.ParseFrom(async_state.Message);

        if (message_packet.HasHeader && message_packet.HasMessage &&
          message_packet.HasService) {

            RubyMessage message = new RubyMessage(message_packet.Header,
              message_packet.Message);

            service_.OnServerMessage(message);
        }
      } catch (InvalidProtocolBufferException iex) {
        logger.Error("[MessageHandler   Nohros.Ruby.Service.Net.RubyNet]   The received protocol buffer message is could not be parsed into a ruby message packet.", iex);
      } catch (Exception ex) {
        // handle exceptions that may occur while the service is handling a
        // message.
        RubyLogger.ForCurrentProcess.Error("[MessageHandler   Nohros.Ruby.Service.Net.RubyNet]   The service " + service_.Name + " was not capable to handle the received message", ex);
      }
    }

    /// <inherithdoc/>
    public IRubyService Service {
      get { return service_; }
    }
  }
}