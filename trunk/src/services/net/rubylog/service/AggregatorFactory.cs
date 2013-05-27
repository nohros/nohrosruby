using System;
using System.Diagnostics;
using Nohros.Configuration;
using Nohros.IO;
using Nohros.Logging;
using ZMQ;

namespace Nohros.Ruby.Logging
{
  public class AggregatorFactory : IRubyServiceFactory
  {
    public IRubyService CreateService(string command_line_string) {
      CommandLine switches = CommandLine.FromString(command_line_string);
      if (switches.HasSwitch(Strings.kDebugSwitch)) {
        Debugger.Launch();
      }

      Settings settings = new Settings.Loader()
        .Load(Path.AbsoluteForCallingAssembly(Strings.kConfigFileName),
          Strings.kConfigRootNodeName);
      ConfigureLogger(settings);
      return CreateAggregator(settings);
    }

    Aggregator CreateAggregator(IAggregatorSettings settings) {
      ILogMessageRepository aggregator_data_provider =
        GetAggregatorDataProvider(settings);
      return new Aggregator(new Context(), settings, aggregator_data_provider);
    }

    ILogMessageRepository GetAggregatorDataProvider(
      IAggregatorSettings settings) {
      IProviderNode provider = settings
        .Providers[string.Empty][Strings.kAggregatorDataProvider];
      return RuntimeTypeFactory<ILogMessageRepositoryFactory>
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
        } catch(System.Exception e) {
          Console.WriteLine();
          Console.WriteLine("[ERROR] - " + e.Message);
        }
      }
    }
  }
}
