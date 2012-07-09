using System;
using Nohros.Logging;
using Nohros.Configuration;
using Nohros.Providers;
using ZMQ;

namespace Nohros.Ruby.Logging
{
  public sealed class AppFactory
  {
    readonly Settings settings_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="AppFactory"/> object using
    /// the specified <see cref="Settings"/> object.
    /// </summary>
    /// <param name="settings">
    /// A <see cref="Settings"/> objects containing the configured application
    /// settings.
    /// </param>
    public AppFactory(Settings settings) {
      settings_ = settings;
    }
    #endregion

    public Aggregator CreateAggergator() {
      Context context = new Context();
      return new Aggregator(
        GetDealerSocket(context), GetPublisherSocket(context), settings_);
    }

    Socket GetDealerSocket(Context context) {
      Socket socket = context.Socket(SocketType.DEALER);
      socket.Bind("tcp://*:" + settings_.ListenerPort);
      return socket;
    }

    Socket GetPublisherSocket(Context context) {
      Socket socket = context.Socket(SocketType.DEALER);
      socket.Bind("tcp://*:" + settings_.PublisherPort);
      return socket;
    }

    public void ConfigureLogger() {
      IProviderNode provider = settings_.Providers[Strings.kLoggingProviderName];
      ILogger logger = ProviderFactory<ILoggerFactory>
        .CreateProviderFactory(provider)
        .CreateLogger(provider.Options, settings_);
      AggregatorLogger.ForCurrentProcess = new AggregatorLogger(logger);
    }
  }
}
