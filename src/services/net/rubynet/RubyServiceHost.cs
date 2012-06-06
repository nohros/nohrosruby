using System;
using System.IO;
using System.IO.Pipes;
using Google.ProtocolBuffers;
using Nohros.Concurrent;

namespace Nohros.Ruby.Service.Net
{
  /// <summary>
  /// .NET implementation of the <see cref="IRubyServiceHost"/> interface. This
  /// class is used to host a .NET based ruby services.
  /// </summary>
  internal class RubyServiceHost : IRubyServiceHost
  {
    const int kTimeout = 30000;
    const int kMessageBufferSize = 4096;

    readonly NamedPipeServerStream ipc_channel_;
    readonly Mailbox<IRubyMessage> mailbox_;
    readonly byte[] message_buffer_;
    readonly IRubyService service_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="RubyServiceHost"/> class
    /// by using the specified ipc channel and service to host.
    /// </summary>
    /// <param name="service">
    /// The service to host.
    /// </param>
    public RubyServiceHost(IRubyService service) {
#if DEBUG
      if (service == null) {
        throw new ArgumentNullException("service");
      }
#endif
      service_ = service;
      message_buffer_ = new byte[kMessageBufferSize];
      mailbox_ = new Mailbox<IRubyMessage>(OnMessage);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RubyServiceHost"/> by
    /// using the specified service and zeromq reply socket.
    /// </summary>
    /// <param name="service">
    /// The service to be hosted by this service host.
    /// </param>
    /// <param name="ipc_channel">
    /// A <see cref="NamedPipeServerStream"/> object that is used on IPC
    /// communication.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="service"/> or <paramref name="ipc_channel"/> is
    /// <c>null</c>
    /// </exception>
    public RubyServiceHost(IRubyService service,
      NamedPipeServerStream ipc_channel) : this(service) {
#if DEBUG
      if (ipc_channel == null) {
        throw new ArgumentNullException("ipc_channel");
      }
#endif
      service_ = service;
      ipc_channel_ = ipc_channel;
    }
    #endregion

    void OnMessage(IRubyMessage message) {
      throw new NotImplementedException();
    }

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
      // We need to start reading the named pipe before the service to ensure
      // that we peek all messages.
      ipc_channel_.BeginRead(message_buffer_, 0, kMessageBufferSize,
        MessageHandler, null);
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
    /// Handle service messages sent from the Ruby Service Host (RSH).
    /// </summary>
    /// <remarks>
    /// The ruby service node communicates with the service host through a
    /// named pipe.The named pipe should be created by the ruby service host
    /// and must have a name with the has the form: \\.\pipe\{PID}, where PID
    /// is the ID of the process that is running the service host. If the
    /// ruby service cannot connect to the named pipe in about 30 seconds, the
    /// service is forced to exit.
    /// <para>
    /// This method receives a message from the ruby server (through the
    /// ruby service node), parse it, and send it to the hosted service.
    /// </para>
    /// </remarks>
    void MessageHandler(IAsyncResult result) {
      IRubyLogger logger = RubyLogger.ForCurrentProcess;

      // Finish the asynchronous operation and get the message.
      int readed = 0;
      try {
        readed = ipc_channel_.EndRead(result);
      } catch (IOException io_exception) {
        #region : logging :
        logger.Error(ipc_channel_.IsConnected
          ? "The stream is closed"
          : "An internal error has occured while reading data from the server.",
          io_exception);
        #endregion
      } catch (Exception exception) {
        #region : logging :
        // Handle exceptions that has occurred during the asynchronous read
        // operation.
        logger.Error("", exception);
        #endregion
      }

      // The pipe has been disconnected.
      if (readed == 0) return;

      // Convert the readed data to an instance of a IRubyMessagePacket and
      // pass them to the service.
      try {
        RubyMessagePacket packet =
          RubyMessagePacket.ParseFrom(
            ByteString.CopyFrom(message_buffer_, 0, readed));

        if (packet.HasHeader && packet.Header.HasService) {
          if (string.Compare(packet.Header.Service, service_.Name,
              StringComparison.OrdinalIgnoreCase) == 0) {
            service_.OnServerMessage(packet.Message);
          }
        }
      } catch (InvalidProtocolBufferException iex) {
        #region : logging :
        logger.Error(
          "The received protocol buffer message is could not be parsed into a" +
            "ruby message packet.", iex);
        #endregion
      } catch (Exception ex) {
        #region : logging :
        logger.Error(
          "The service was not capable to handle the received message", ex);
        #endregion
      }
    }

    /// <inherithdoc/>
    public IRubyService Service {
      get { return service_; }
    }
  }
}
