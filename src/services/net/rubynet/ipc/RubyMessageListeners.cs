using System;
using Nohros.Ruby.Protocol;

namespace Nohros.Ruby
{
  public static class RubyMessageListeners
  {
    class RubyMessageListenerFromDelegate : IRubyMessageListener
    {
      readonly Action<RubyMessagePacket> action_;

      #region .ctor
      public RubyMessageListenerFromDelegate(Action<RubyMessagePacket> action) {
        action_ = action;
      }
      #endregion

      /// <inheritdoc/>
      public void OnMessagePacketReceived(RubyMessagePacket packet) {
        action_(packet);
      }
    }

    /// <summary>
    /// Creates a new instance of the <see cref="IRubyMessageListener"/> class
    /// that executes the <paramref name="action"/> when a
    /// <see cref="IRubyMessageListener.OnMessagePacketReceived"/> is called.
    /// </summary>
    /// <param name="action">
    /// A <see cref="Action{T}"/> delegate that represents the method that will
    /// be executed when
    /// <see cref="IRubyMessageListener.OnMessagePacketReceived"/> is called.
    /// </param>
    /// <returns></returns>
    public static IRubyMessageListener FromDelegate(Action<RubyMessagePacket> action) {
      return new RubyMessageListenerFromDelegate(action);
    }
  }
}
