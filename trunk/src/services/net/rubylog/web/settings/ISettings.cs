using System;
using Nohros.Configuration;

namespace Nohros.Ruby.Logging
{
  public interface ISettings : IConfiguration
  {
    /// <summary>
    /// Gets the address of the log publisher server.
    /// </summary>
    string PublisherEndpoint { get; }
  }
}