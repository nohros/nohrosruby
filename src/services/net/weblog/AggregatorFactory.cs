using System;
using ZMQ;

namespace Nohros.Ruby.Logging
{
  public class AggregatorFactory : IRubyServiceFactory
  {
    public IRubyService CreateService(string command_line_string) {
      AppFactory app = new AppFactory();
      return CreateAggregator(app.LoadSettings());
    }

    public Aggregator CreateAggregator(IAggregatorSettings settings) {
      return new Aggregator(new Context(), settings);
    }
  }
}
