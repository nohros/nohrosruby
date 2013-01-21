using System;
using Google.ProtocolBuffers;
using Nohros.Concurrent;

namespace Nohros.Ruby
{
  public struct QueryRequestFuture
  {
    readonly AsyncCallback callback_;
    readonly SettableFuture<byte[]> response_;
    readonly object state_;

    #region .ctor
    public QueryRequestFuture(SettableFuture<byte[]> response,
      AsyncCallback callback,
      object state) {
      response_ = response;
      callback_ = callback;
      state_ = state;
    }
    #endregion

    public SettableFuture<byte[]> Response {
      get { return response_; }
    }

    public AsyncCallback Callback {
      get { return callback_; }
    }
  }
}
