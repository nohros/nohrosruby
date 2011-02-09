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
    /// ruby .NET based services.
    /// </summary>
    internal class RubyNetServiceHost : IRubyServiceHost
    {
        const int kTimeout = 30000;

        string ipc_channel_name_;
        IRubyService service_;
        ManualResetEvent mutex_;
        NamedPipeClientStream pipe_stream_;

        #region .ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="RubyNetServiceHost"/> by using the specified
        /// service and IPC channel name.
        /// </summary>
        public RubyNetServiceHost(IRubyService service, string ipc_channel_name) {
            service_ = service;
            mutex_ = new ManualResetEvent(false);
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
        /// </remarks>
        public void StartService() {
            // attempt to open the IPC channel.
            if (ipc_channel_name_ != null && ipc_channel_name_ != string.Empty)
                ConnectPipe(ipc_channel_name_);

            Thread service_thread = new Thread(new ThreadStart(delegate() {
                // start the service.
                service_.Run();

                // signal the main thread that the service has been finished.
                mutex_.Set();
            }));

            service_thread.IsBackground = true;
            service_thread.Start();

            // wait the service to finish.
            mutex_.WaitOne();

            // releasing resources
            if (pipe_stream_ != null && pipe_stream_.IsConnected) {
                pipe_stream_.Close();
                pipe_stream_ = null;
            }
        }

        /// <summary>
        /// Connects to a named pipe whose name is <see cref="pipe_name"/>.
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
        /// <param name="service">The <see cref="IRubyService"/> object that will receive the messages</param>
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

            // TODO: convert the readed data to an instance of a IRubyMessage and pass them to the service.
            
            //service_.OnServerMessage();
        }

        public bool SendMessage(IRubyMessage message) {
            return false;
        }
    }
}
