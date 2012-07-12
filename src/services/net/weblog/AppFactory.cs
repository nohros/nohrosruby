using System;
using System.IO;
using System.Reflection;
using Nohros.Logging;
using Nohros.Configuration;
using Nohros.Providers;

namespace Nohros.Ruby.Logging
{
  public sealed class AppFactory
  {
    public Settings LoadSettings() {
      string assembly_location = Assembly.GetExecutingAssembly().Location;
      string config_file_path =
        Path.Combine(Path.GetDirectoryName(assembly_location),
          Strings.kConfigFileName);

      Settings settings = new Settings();
      settings.Load(config_file_path, Strings.kRootFileName);
      ConfigureLogger(settings);
      settings.AggregatorDataProvider = GetAggregatorDataProvider(settings);
      return settings;
    }

    IAggregatorDataProvider GetAggregatorDataProvider(IAggregatorSettings settings) {
      IProviderNode provider =
        settings.Providers[Strings.kAggregatorDataProvider];
      return
        ProviderFactory<IAggregatorDataProviderFactory>
          .CreateProviderFactoryFallback(provider, settings)
          .CreateAggregatorDataProvider(provider.Options);
    }

    void ConfigureLogger(ISettings settings) {
      IProviderNode provider = settings.Providers[Strings.kLoggingProviderName];
      ILogger logger = ProviderFactory<ILoggerFactory>
        .CreateProviderFactory(provider)
        .CreateLogger(provider.Options, settings);
      RubyLogger.ForCurrentProcess = new RubyLogger(logger);
    }
  }
}
