using System;
using Nohros.Configuration;
using Nohros.IO;
using Nohros.Logging;
using ZMQ;

namespace Nohros.Ruby.Logging
{
  public class AggregatorFactory : IRubyServiceFactory
  {
    public IRubyService CreateService(string command_line_string) {
      Settings settings = new Settings.Loader()
        .Load(Path.AbsoluteForCallingAssembly(string.Empty),
          Strings.kConfigRootNodeName);
      ConfigureLogger(settings);
      return CreateAggregator(settings);
    }

    Aggregator CreateAggregator(IAggregatorSettings settings) {
      IAggregatorDataProvider aggregator_data_provider =
        GetAggregatorDataProvider(settings);
      return new Aggregator(new Context(), settings, aggregator_data_provider);
    }

    IAggregatorDataProvider GetAggregatorDataProvider(
      IAggregatorSettings settings) {
      IProviderNode provider = settings
        .Providers[string.Empty][Strings.kAggregatorDataProvider];
      return RuntimeTypeFactory<IAggregatorDataProviderFactory>
        .CreateInstanceFallback(provider, settings)
        .CreateAggregatorDataProvider(provider.Options.ToDictionary());
    }

    void ConfigureLogger(ISettings settings) {
      IProviderNode provider;
      IProvidersNodeGroup providers = settings
        .Providers[String.Empty];
      if (providers.GetProviderNode(Strings.kLoggingProviderName, out provider)) {
        try {
          LocalLogger.ForCurrentProcess.Logger =
            RuntimeTypeFactory<ILoggerFactory>
              .CreateInstanceFallback(provider, settings)
              .CreateLogger(provider.Options.ToDictionary());
        } catch {
        }
      }
    }
  }
}
