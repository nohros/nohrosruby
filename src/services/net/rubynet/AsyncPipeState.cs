using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Pipes;

namespace Nohros.Ruby.Service.Net
{
    internal class AsyncPipeState
    {
        const int kMaxBufferLength = 4024;

        byte[] message_;
        NamedPipeClientStream pipe_stream_;
        IRubyService service_;

        #region .ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncPipeResult"/> by using the specified
        /// <see cref="NamedPipeClientStream"/> and <see cref="IRubyService"/>.
        /// </summary>
        public AsyncPipeState(NamedPipeClientStream pipe_stream, IRubyService service) {
            message_ = new byte[kMaxBufferLength];
            pipe_stream_ = pipe_stream;
            service_ = service;
        }
        #endregion

        /// <summary>
        /// Gets an byte array containing the message sent from the ruby agent service.
        /// </summary>
        /// <value>A byte array where the data sent from the agent service agent will be read into.</value>
        /// <remarks>This property will be used by the named pipe to read data into.</remarks>
        public byte[] Message {
            get { return message_; }
        }

        /// <summary>
        /// Gets the pipe related with the asynchronous operation.
        /// </summary>
        public NamedPipeClientStream PipeStream {
            get { return pipe_stream_; }
        }

        /// <summary>
        /// Gets the service related with the asynchronous operation.
        /// </summary>
        public IRubyService RubyService {
            get { return service_; }
        }
    }
}
