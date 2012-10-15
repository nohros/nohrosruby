using System;

namespace Nohros.Ruby.Logging
{
  public interface IAggregatorSettings : ISettings
  {
    /// <summary>
    /// Gets the port number that is used by the aggregator to publish messages.
    /// </summary>
    int PublisherPort { get; }
  }
}
