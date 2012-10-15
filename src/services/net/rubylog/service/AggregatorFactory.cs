using System;
using Nohros.IO;
using ZMQ;

namespace Nohros.Ruby.Logging
{
  public class AggregatorFactory : IRubyServiceFactory
  {
    public IRubyService CreateService(string command_line_string) {
      Settings settings = new Settings.Loader()
        .Load(Path.AbsoluteForCallingAssembly(string.Empty),
          Strings.kConfigRootNodeName);
      return CreateAggregator(settings);
    }

    public Aggregator CreateAggregator(IAggregatorSettings settings) {
      return new Aggregator(new Context(), settings);
    }
  }
}
