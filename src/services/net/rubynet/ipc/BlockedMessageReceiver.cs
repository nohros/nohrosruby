﻿using System;
using System.Threading;

namespace Nohros.Ruby
{
  /// <summary>
  /// An implementation of the <see cref="IRubyMessageListener"/> interface
  /// that blocks until the application exit.
  /// </summary>
  public class BlockedMessageReceiver: IRubyMessageReceiver, IDisposable
  {
    readonly AutoResetEvent waiter_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="BlockedMessageReceiver"/>
    /// class.
    /// </summary>
    public BlockedMessageReceiver() {
      waiter_ = new AutoResetEvent(false);
    }
    #endregion

    /// <inheritdoc/>
    public RubyMessagePacket GetMessagePacket() {
      waiter_.WaitOne();
      return new RubyMessagePacket.Builder().SetSize(0).Build();
    }

    public void Dispose() {
      waiter_.Set();
    }
  }
}
