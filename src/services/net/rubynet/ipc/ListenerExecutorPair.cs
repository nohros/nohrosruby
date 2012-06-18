using System;
using Nohros.Concurrent;

namespace Nohros.Ruby
{
  internal partial class IPCChannel
  {
    class ListenerExecutorPair {
      readonly IRubyMessageListener listener_;
      readonly IExecutor executor_;

      /// <summary>
      /// Initializes a new instance of the <see cref="ListenerExecutorPair"/>
      /// class by using the specified listener and executor.
      /// </summary>
      public ListenerExecutorPair(IRubyMessageListener listener, IExecutor executor) {
        listener_ = listener;
        executor_ = executor;
      }
      
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
      public IRubyMessageListener Listener {
        get { return listener_; }
      }
    }
  }
}
