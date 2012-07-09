using System;
using Nohros.Configuration;

namespace Nohros.Ruby.Logging
{
  public interface ISettings : IMustConfiguration
  {
    /// <summary>
    /// Gets an instance of the <see cref="IAggregatorDataProvider"/> class
    /// that is used to persit the logged messages.
    /// </summary>
    IAggregatorDataProvider AggregatorDataProvider { get; }
  }
}
