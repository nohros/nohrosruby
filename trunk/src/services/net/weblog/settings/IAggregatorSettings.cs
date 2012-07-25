using System;

namespace Nohros.Ruby.Logging
{
  public interface IAggregatorSettings : ISettings
  {
    /// <summary>
    /// Gets the port number that is used by the aggregator to publish messages.
    /// </summary>
    int PublisherPort { get; }

    /// <summary>
    /// Gets a value indicating if the service is self hosting.
    /// </summary>
    /// <remarks>
    /// A service is self hosted when it does not uses the ruby node service
    /// infrastructure. For example: when the .NET ruby service host is started
    /// at interactive mode.
    /// </remarks>
    bool SelfHost { get; }

    /// <summary>
    /// Gets the port number that should be used to listen form messages when
    /// we are self hosting the service.
    /// </summary>
    int SelfHostPort { get; }
  }
}
