using System;
using Nohros.Concurrent;

namespace Nohros.Ruby
{
  internal partial class RubyMessageChannel
  {
    class ListenerExecutorPair
    {
      readonly IRubyMessageListener listener_;
      readonly IExecutor executor_;
      readonly string service_;

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
      /// <param name="service">
      /// The name of the service associated with the listener.
      /// </param>
      /// <remarks>
      /// Each listener should be associated with a service(real or virtual).
      /// It will receive only the messages that is destinated to the
      /// associated service.
      /// </remarks>
      public ListenerExecutorPair(IRubyMessageListener listener, IExecutor executor, string service) {
        listener_ = listener;
        executor_ = executor;
        service_ = service;
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

      /// <summary>
      /// Gets the name of the service associated with the pair.
      /// </summary>
      public string Service {
        get { return service_; }
      }
    }
  }
}
