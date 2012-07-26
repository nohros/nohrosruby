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
    readonly ILogger logger_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="AggregatorService"/>
    /// class by using the specified <see cref="ILogger"/> interface.
    /// </summary>
    /// <param name="context">
    /// The zeromq context used to manage sockets.
    /// </param>
    /// <param name="log_aggregator_address">
    /// The address of the aggregator service (ip:port).
    /// </param>
    /// <param name="logger">
    /// A <see cref="ILogger"/> object that is used a fallback logging
    /// mechanism.
    /// </param>
    /// <remarks>
    /// The fall back logger is explicit required because the singleton
    /// RubyLogger.ForCurrentProcess also uses the
    /// <see cref="AggregatorService"/> class.
    /// </remarks>
    public AggregatorService(Context context, string log_aggregator_address,
      ILogger logger) {
      context_ = context;
      dealer_ = context_.Socket(SocketType.DEALER);
      log_aggregator_address_ = log_aggregator_address;
      logger_ = logger;
    }
    #endregion

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
  }
}
