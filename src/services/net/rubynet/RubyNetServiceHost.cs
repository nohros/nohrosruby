using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;
using System.IO.Pipes;

using Google.ProtocolBuffers;

namespace Nohros.Ruby.Service.Net
{
    /// <summary>
    /// .NET implementation of the <see cref="IRubyServiceHost"/>. This class is used to host
    /// .NET based ruby services.
    /// </summary>
    internal class RubyNetServiceHost : IRubyServiceHost
    {
        const int kTimeout = 30000;

        string ipc_channel_name_;
        IRubyService service_;
        NamedPipeClientStream pipe_stream_;

        static EmptyServiceHost empty_service_host_;

        #region .ctor
        static RubyNetServiceHost() {
            empty_service_host_ = new EmptyServiceHost();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RubyNetServiceHost"/> by using the specified
        /// service and IPC channel name.
        /// </summary>
        public RubyNetServiceHost(IRubyService service, string ipc_channel_name) {
            service_ = service;
            ipc_channel_name_ = ipc_channel_name;
        }
        #endregion

        /// <summary>
        /// Starts the hosted service.
        /// </summary>
        /// <remarks>
        /// <para>The hosted service runs into a dedicated thread. The thread where this code is running is
        /// used to send/receive messages to/from the service.</para>
        /// <para>This method does not return until the running hosted service have finished your execution.</para>
        /// <para>If the service throws any exception this is propaggated to the caller and the status
        /// of service is changed to stopped.</para>
        /// </remarks>
        public void StartService() {
            // attempt to open the IPC channel.
            if (ipc_channel_name_ != null && ipc_channel_name_ != string.Empty)
                ConnectPipe(ipc_channel_name_);

            // runs the service.
            service_.Run();
        }

        /// <summary>
        /// Stops the hosted service.
        /// </summary>
        /// <remarks>
        /// </remarks>
        public void StopService() {
        }

        /// <summary>
        /// Connects to a named pipe whose name is <paramref name="pipe_name"/>
        /// </summary>
        /// <param name="pipe_name">The name of the pipe to connects.</param>
        /// <returns>true if the connection is successfull; otherwise, false.</returns>
        /// <remarks>The communication between the service agent and the service host is done through
        /// a named pipe. The communication link could be estabilished/desestablished at any time. This is done
        /// by the special named-pipe</remarks>
        bool ConnectPipe(string pipe_name) {
            try {
                pipe_stream_ = new NamedPipeClientStream(pipe_name);
                pipe_stream_.Connect(kTimeout);

                // the pipe transmission mode must be message
                if (pipe_stream_.TransmissionMode == PipeTransmissionMode.Byte) {
                    RubyNet.LOG_ERROR("[ConnectPipe   Nohros.Ruby.Service.Net.RubyNetServiceHost]   The pipe transmission mode must be message(PIPE_TYPE_MESSAGE).");
                    return false;
                }

                // we need to read and write to the pipe.
                if (!pipe_stream_.CanRead || !pipe_stream_.CanWrite) {
                    RubyNet.LOG_ERROR("[ConnectPipe   Nohros.Ruby.Service.Net.RubyNetServiceHost]   The pipe is not writable or readable.");
                    return false;
                }

                AsyncPipeState state = new AsyncPipeState(pipe_stream_, service_);
                pipe_stream_.BeginRead(state.Message, 0, state.Message.Length, MessageHandler, state);

                return true;
            } catch (Exception exception) {
                RubyNet.LOG_ERROR("[ConnectPipe   Nohros.Ruby.Service.Net.RubyNetServiceHost]", exception);
            }
            return false;
        }

        /// <summary>
        /// Handle service messages sent from the Ruby Service Host.
        /// </summary>
        /// <remarks>
        /// The ruby agent communicates with the service host through a named pipe. Each service host must create a 
        /// pipe with the name that was specified by the ruby agent service and wait for the connection of the agent.
        /// <para>
        /// This method receives a message from the ruby server, parse it, packet it into a new <see cref="IRubyService"/>
        /// object and send it to the hosted service.
        /// </para>
        /// </remarks>
        void MessageHandler(IAsyncResult result) {
            AsyncPipeState async_state = result.AsyncState as AsyncPipeState;
            NamedPipeClientStream pipe_stream = async_state.PipeStream;

            // ends the asynchronous operation and get the results.
            int readed = 0;
            try {
                readed = pipe_stream_.EndRead(result);
            } catch (IOException io_exception) {
                RubyNet.LOG_ERROR("[MessageHandler   Nohros.Ruby.Service.Net.RubyNet]   " +
                    ((pipe_stream.IsConnected) ? "The stream is closed" : "an internal error has occured while reading data from the server."), io_exception);
            } catch (Exception exception) {
                // handle exceptions that has occurred during the asynchronous read operation.
                RubyNet.LOG_ERROR("[MessageHandler   Nohros.Ruby.Service.Net.RubyNet]", exception);
            }

            // the pipe is disconnected
            if (readed == 0) return;

            // convert the readed data to an instance of a IRubyMessagePacket and pass them to the service.
            try {
                RubyMessagePacket message_packet = RubyMessagePacket.ParseFrom(async_state.Message);
                service_.OnServerMessage(message_packet);
            } catch (InvalidProtocolBufferException iex) {
                RubyNet.LOG_ERROR("[MessageHandler   Nohros.Ruby.Service.Net.RubyNet]   The received protocol buffer message is could not be parsed into a ruby message packet.", iex);
            } catch(Exception ex) {
                // handle exceptions that may occur while the service is handling a message.
                RubyNet.LOG_ERROR("[MessageHandler   Nohros.Ruby.Service.Net.RubyNet]   The service " + service_.Name + " was not capable to handle the received message", ex);
            }
        }

        public bool SendMessage(IRubyMessagePacket message) {
            return false;
        }

        public static EmptyServiceHost EmptyServiceHost {
            get { return empty_service_host_; }
        }

        /// <inherithdoc/>
        public IRubyService Service {
            get { return service_; }
        }
    }
}
