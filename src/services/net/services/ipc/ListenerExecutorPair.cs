using System;
using Nohros.Concurrent;

namespace Nohros.Ruby
{
  internal class ListenerExecutorPair
  {
    readonly IExecutor executor_;
    readonly IRubyMessagePacketListener listener_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="ListenerExecutorPair"/>
    /// class by using the specified listener and executor.
    /// </summary>
    /// <param name="listener">
    /// A <see cref="IRubyMessageListener"/> that should be called when a
    /// message is received by the channel.
    /// </param>
    /// <param name="executor">
    /// A <see cref="IExecutor"/> object that is used to execute the
    /// <see cref="IRubyMessageListener.OnMessagePacketReceived"/> callback.
    /// </param>
    /// <remarks>
    /// Each listener should be associated with a service(real or virtual).
    /// It will receive only the messages that is destinated to the
    /// associated service.
    /// </remarks>
    public ListenerExecutorPair(IRubyMessagePacketListener listener,
      IExecutor executor) {
      listener_ = listener;
      executor_ = executor;
    }
    #endregion

    /// <summary>
    /// Gets the <see cref="IExecutor"/> object associated with the pair.
    /// </summary>
    public IExecutor Executor {
      get { return executor_; }
    }

    /// <summary>
    /// gets the <see cref="IRubyMessageListener"/> object associated with
    /// the pair.
    /// </summary>
    public IRubyMessagePacketListener Listener
    {
      get { return listener_; }
    }
  }
}
