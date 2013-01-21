using System;
using System.Collections.Generic;
using System.Threading;
using Google.ProtocolBuffers;
using Nohros.Concurrent;
using Nohros.Resources;
using Nohros.Ruby.Protocol;
using ZMQ;
using ZException = ZMQ.Exception;
using Exception = System.Exception;

namespace Nohros.Ruby
{
  /// <summary>
  /// A <see cref="QueryApplication"/> is a class that ca be used to
  /// asynchronously handle the comunication with a ruby service.
  /// </summary>
  public class QueryApplication : IDisposable
  {
    const string kClassName = "Nohros.Ruby.QueryApplication";
    readonly Context context_;
    readonly Dictionary<string, QueryRequestFuture> futures_;
    readonly QueryLogger logger_;
    readonly string query_server_address_;
    readonly IThreadFactory receiver_thread_factory_;
    readonly Socket socket_;
    Thread receiver_thread_;
    bool running_;

    #region .ctor
    public QueryApplication(Context context, string query_server_address,
      IThreadFactory receiver_thread_factory) {
      socket_ = context.Socket(SocketType.DEALER);
      receiver_thread_factory_ = receiver_thread_factory;
      context_ = context;
      running_ = false;
      futures_ = new Dictionary<string, QueryRequestFuture>();
      logger_ = QueryLogger.ForCurrentProcess;
      query_server_address_ = query_server_address;
    }
    #endregion

    public void Dispose() {
      Stop();
      socket_.Dispose();
    }

    public IFuture<byte[]> ProcessQuery(QueryRequest request,
      AsyncCallback callback) {
      return ProcessQuery(request, callback, null);
    }

    public IFuture<byte[]> ProcessQuery(QueryRequest request,
      AsyncCallback callback, object state) {
      RubyMessagePacket packet = GetMessagePacket(request);
      try {
        SettableFuture<byte[]> future =
          new SettableFuture<byte[]>(state);
        futures_.Add(Convert.ToBase64String(request.ID),
          new QueryRequestFuture(future, callback, state));

        // Send the request and wait for the response. The request should
        // follow the REQ/REP pattern, which contains the following parts:
        //   1. [EMPTY FRAME]
        //   2. [MESSAGE]
        // 
        socket_.SendMore();
        socket_.Send(packet.ToByteArray());
        return future;
      } catch (Exception e) {
        return Futures.ImmediateFailedFuture<byte[]>(e);
      }
    }

    /// <summary>
    /// Generates a <see cref="ExceptionMessage"/> for the given
    /// <see cref="Exception"/>.
    /// </summary>
    /// <param name="request">
    /// The request that originates the exception.
    /// </param>
    /// <param name="code">
    /// The code of the exception.
    /// </param>
    /// <param name="exception">
    /// The exception that was produced.
    /// </param>
    /// <returns>
    /// A <see cref="ByteString"/> representing the supplied exception.
    /// </returns>
    byte[] FormatException(QueryRequest request, StatusCode code,
      Exception exception) {
      return new ExceptionMessage.Builder()
        .SetCode((int) code)
        .SetMessage(exception.Message)
        .SetSource(exception.Source)
        .AddData(KeyValuePairs.FromKeyValuePair("backtrace",
          exception.StackTrace))
        .AddData(KeyValuePairs.FromKeyValuePair("requestId",
          Convert.ToBase64String(request.ID)))
        .AddData(KeyValuePairs.FromKeyValuePair("messageToken",
          request.MessageToken))
        .AddData(KeyValuePairs.FromKeyValuePair("messageType",
          request.MessageType.ToString()))
        .Build()
        .ToByteArray();
    }

    void ProcessResponse(byte[] response) {
      try {
        var packet = RubyMessagePacket.ParseFrom(response);
        byte[] request_id = packet.Message.Id.ToByteArray();
        string base64_request_id = Convert.ToBase64String(request_id);
        QueryRequestFuture request;
        if (futures_.TryGetValue(base64_request_id, out request)) {
          futures_.Remove(base64_request_id);
        }
      } catch (Exception exception) {
        logger_.Error(
          string.Format(StringResources.Log_MethodThrowsException,
            "ProcessResponse", kClassName), exception);
      }
    }

    void GetResponse() {
      while (running_) {
        Queue<byte[]> parts = socket_.RecvAll();
        if (parts.Count != 2) {
          // The response should follow the REQ/REP pattern, which contains
          // the following parts.
          //   1. [EMPTY FRAME]
          //   2. [MESSAGE]
          //
          logger_.Error(Resources.Socket_ReceivedTooManyParts);
          socket_.RecvAll();
        } else {
          // discard the empty frame and process the response.
          parts.Dequeue();
          ProcessResponse(parts.Dequeue());
        }
      }
    }

    RubyMessagePacket GetMessagePacket(QueryRequest request) {
      RubyMessage msg = new RubyMessage.Builder()
        .SetId(ByteString.CopyFrom(request.ID))
        .SetAckType(RubyMessage.Types.AckType.kRubyNoAck)
        .SetType(request.MessageType)
        .SetToken(request.MessageToken)
        .SetMessage(request.Message)
        .Build();

      RubyMessageHeader header = new RubyMessageHeader.Builder()
        .SetId(msg.Id)
        .SetSize(msg.SerializedSize)
        .Build();

      return new RubyMessagePacket.Builder()
        .SetMessage(msg)
        .SetHeader(header)
        .SetHeaderSize(header.SerializedSize)
        .SetSize(header.SerializedSize + header.Size)
        .Build();
    }

    /// <summary>
    /// Starts the application.
    /// </summary>
    public void Start() {
      socket_.Connect(Transport.TCP, query_server_address_);
      receiver_thread_ = receiver_thread_factory_
        .CreateThread(GetResponse);
      running_ = true;
      receiver_thread_.Start();
    }

    /// <summary>
    /// Stops the application.
    /// </summary>
    public void Stop() {
      futures_.Clear();
      if (receiver_thread_ != null) {
        // forces the socket to close.
        socket_.Dispose();

        // wait the main thread to finish its work.
        receiver_thread_.Join();
      }
    }
  }
}
