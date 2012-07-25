using System;
using Nohros.Ruby.Logging;
using ZMQ;
using Nohros.Logging;
using R = Nohros.Resources;

namespace Nohros.Ruby
{
  internal class AggregatorService : IAggregatorService
  {
    const string kClassName = "Nohros.Ruby.AggregatorService";

    readonly Context context_;
    readonly Socket dealer_;
    readonly string log_aggregator_address_;
    readonly IRubyLogger logger_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="AggregatorService"/>
    /// class by using the specified <see cref="ILogger"/> interface.
    /// </summary>
    public AggregatorService(Context context, string log_aggregator_address) {
      context_ = context;
      dealer_ = context_.Socket(SocketType.DEALER);
      log_aggregator_address_ = log_aggregator_address;
      logger_ = RubyLogger.ForCurrentProcess;
    }
    #endregion

    /// <summary>
    /// Configures the logger.
    /// </summary>
    /// <remarks>
    /// This method should be called to make the logger usable.
    /// </remarks>
    /// <returns>
    /// <c>true</c> when the logger was successfully configured; otherwise,
    /// <c>false</c>.
    /// </returns>
    public bool Configure() {
      try {
        dealer_.Connect("tcp://" + log_aggregator_address_);
      } catch (System.Exception exception) {
        logger_.Error(
          string.Format(R.StringResources.Log_MethodThrowsException, kClassName,
            "Configure"), exception);
        return false;
      }
      return true;
    }

    /// <summary>
    /// Sends the specified log message to the log aggregator.
    /// </summary>
    /// <param name="log">The message to log.</param>
    public void Log(LogMessage log) {
      try {
        dealer_.Send(log.ToByteArray());
      } catch (System.Exception exception) {
        logger_.Error(
          string.Format(R.StringResources.Log_MethodThrowsException, kClassName,
            "Log"), exception);
      }
    }
  }
}
