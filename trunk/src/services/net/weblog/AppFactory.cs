using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Reflection;

using Nohros.Logging;
using Nohros.Configuration;
using Nohros.Providers;
using ZMQ;

namespace Nohros.Ruby.Weblog
{
  public sealed class AppFactory
  {
    public WeblogSettings CreateSettings() {
      string assembly_location = Assembly.GetExecutingAssembly().Location;
      string config_file_path =
        Path.Combine(Path.GetDirectoryName(assembly_location),
          Strings.kConfigFileName);

      WeblogSettings settings = new WeblogSettings();
      settings.Load(config_file_path, Strings.kRootFileName);
      return settings;
    }

    public Aggregator CreateAggergator(WeblogSettings settings) {
      Context context = new Context();
      ZMQ.Socket subscriber = context.Socket(ZMQ.SocketType.SUB);
      TcpListener listener = new TcpListener(IPAddress.Any,
        settings.PublisherPort);

      return new Aggregator(listener.Server, subscriber);
    }

    void CreateLogger(WeblogSettings settings) {
      IProviderNode provider = settings.Providers[Strings.kLoggingProviderName];
      ILogger logger = ProviderFactory<ILoggerFactory>
        .CreateProviderFactory(provider)
        .CreateLogger(provider.Options, settings);
      WeblogLogger.ForCurrentProcess = new WeblogLogger(logger);
    }
  }
}
