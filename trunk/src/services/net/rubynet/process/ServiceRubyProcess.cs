using System;
using System.Threading;
using Nohros.Ruby.Protocol;

namespace Nohros.Ruby
{
  /// <summary>
  /// A <see cref="IRubyProcess"/> that runs as a pseudo windows service.
  /// </summary>
  internal class ServiceRubyProcess : AbstractRubyProcess,
                                      IRubyMessagePacketListener
  {
    readonly IRubyMessageChannel ruby_message_channel_;
    readonly ManualResetEvent manual_reset_event_;
    readonly RubyLogger logger_;
    Thread main_thread_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceRubyProcess"/>
    /// class.
    /// </summary>
    public ServiceRubyProcess(IRubySettings settings,
      IRubyMessageChannel ruby_message_channel)
      : base(settings, ruby_message_channel) {
      ruby_message_channel_ = ruby_message_channel;
      manual_reset_event_ = new ManualResetEvent(false);
      logger_ = RubyLogger.ForCurrentProcess;
    }
    #endregion

    /// <inheritdoc/>
    void IRubyMessagePacketListener.OnMessagePacketReceived(
      RubyMessagePacket packet) {
    }

    /// <inheritdoc/>
    public override void Run(string command_line_string) {
      base.Run(command_line_string);
      
      main_thread_ = Thread.CurrentThread;

      ruby_message_channel_.Open();
      
      QueryLogAggregatorService();
      Syn();
      
      logger_.Info("Service process has ben started. Waiting for incoming messages...");

      manual_reset_event_.WaitOne();
    }

    /// <inheritdoc/>
    public override void Exit() {
      manual_reset_event_.Set();
      if (main_thread_ != null) {
        main_thread_.Join();
      }
      logger_.Info("Service process has been finished.");
    }
  }
}
