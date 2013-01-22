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
    readonly Socket socket_;
    readonly object sync_;
    Thread receiver_thread_;
    bool running_;

    #region .ctor
    public QueryApplication(Context context, string query_server_address) {
      socket_ = context.Socket(SocketType.DEALER);
      context_ = context;
      running_ = false;
      futures_ = new Dictionary<string, QueryRequestFuture>();
      logger_ = QueryLogger.ForCurrentProcess;
      query_server_address_ = query_server_address;
      sync_ = new object();
    }
    #endregion

    public void Dispose() {
      Stop();
      socket_.Dispose();
    }

    public IFuture<byte[]> ExecuteQuery(QueryRequest request) {
      return ExecuteQuery(request, Timeout.Infinite);
    }

    public IFuture<byte[]> ExecuteQuery(QueryRequest request, int timeout) {
      return ExecuteQuery(request, timeout, delegate { });
    }

    public IFuture<byte[]> ExecuteQuery(QueryRequest request,
      AsyncCallback callback) {
      return ExecuteQuery(request, Timeout.Infinite, callback);
    }

    public IFuture<byte[]> ExecuteQuery(QueryRequest request,
      int timeout, AsyncCallback callback) {
      return ExecuteQuery(request, timeout, callback, null);
    }

    public IFuture<byte[]> ExecuteQuery(QueryRequest request,
      AsyncCallback callback, object state) {
      return ExecuteQuery(request, Timeout.Infinite, callback, state);
    }

    public IFuture<byte[]> ExecuteQuery(QueryRequest request, int timeout,
      AsyncCallback callback, object state) {
      if (callback == null || request == null) {
        throw new ArgumentNullException(callback == null
          ? "callback"
          : "request");
      }

      if (timeout != Timeout.Infinite && timeout < 0) {
        throw new ArgumentOutOfRangeException(
          StringResources.ArgumentOutOfRange_NeedNonNegNum);
      }

      RubyMessagePacket packet = GetMessagePacket(request);
      try {
        SettableFuture<byte[]> future =
          new SettableFuture<byte[]>(state);
        string base64_request_id = Convert.ToBase64String(request.ID);
        QueryRequestFuture response = new QueryRequestFuture(future, callback,
          state);

        lock (sync_) {
          futures_.Add(base64_request_id, response);
        }

        // Set the timeout before send the message to ensure that the
        // response comes before the timeout definition.
        if (timeout != Timeout.Infinite) {
          WaitHandle waiter = ((IAsyncResult) future).AsyncWaitHandle;
          ThreadPool.RegisterWaitForSingleObject(waiter,
            delegate(object obj, bool timed_out) {
              IsTimedOut(timed_out, ((QueryRequestFuture) obj).Callback,
                base64_request_id);
            }, response, timeout, true);
        }

        // Send the request and wait for the response. The request should
        // follow the REQ/REP pattern, which contains the following parts:
        //   1. [EMPTY FRAME]
        //   2. [MESSAGE]
        //
        socket_.SendMore();
        socket_.Send(packet.ToByteArray());
        return future;
      } catch (Exception e) {
        logger_.Error(
          string.Format(StringResources.Log_MethodThrowsException,
            "ProcessResponse", kClassName), e);
        return Futures.ImmediateFailedFuture<byte[]>(e);
      }
    }

    void IsTimedOut(bool timed_out, AsyncCallback callback,
      string base64_request_id) {
      QueryRequestFuture future;
      if (timed_out && futures_.TryGetValue(base64_request_id, out future)) {
        bool removed;
        lock (sync_) {
          // Removes the future from the futures collection inside the lock
          // to prevent the callback to be executed more than once, in the case
          // that the request completes while we are evaluating the timeout.
          removed = futures_.Remove(base64_request_id);
        }
        if (removed) {
          future.Response.SetException(new TimeoutException(), false);
          callback(future.Response);
        }
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
      var packet = RubyMessagePacket.ParseFrom(response);
      byte[] request_id = packet.Message.Id.ToByteArray();
      string base64_request_id = Convert.ToBase64String(request_id);
      QueryRequestFuture request;
      if (futures_.TryGetValue(base64_request_id, out request)) {
        bool removed;
        lock (sync_) {
          removed = futures_.Remove(base64_request_id);
        }
        if (removed) {
          request.Response.Set(response, false);
          request.Callback(request.Response);
        }
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
        .SetMessage(ByteString.CopyFrom(request.Message))
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
    /// Runs the application.
    /// </summary>
    /// <remarks>
    /// <see cref="Run()"/> method blocks the calling thread until
    /// <see cref="Stop"/> is called.
    /// </remarks>
    public void Run() {
      socket_.Connect(query_server_address_);
      receiver_thread_ = Thread.CurrentThread;
      running_ = true;
      GetResponse();
    }

    /// <summary>
    /// Runs the application.
    /// </summary>
    /// <remarks>
    /// <see cref="Run(IExecutor)"/> method blocks the executor thread until
    /// <see cref="Stop"/> is called.
    /// </remarks>
    public void Run(IExecutor executor) {
      executor.Execute(Run);
    }

    /// <summary>
    /// Stops the application.
    /// </summary>
    public void Stop() {
      if (receiver_thread_ != null) {
        // forces the socket to close.
        socket_.Dispose();

        // Give some time to the main thread to finish its work.
        if (!receiver_thread_.Join(30*1000)) {
          receiver_thread_.Abort();
        }
      }
    }
  }
}
