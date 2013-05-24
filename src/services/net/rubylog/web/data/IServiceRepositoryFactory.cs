using System;
using System.Collections.Generic;

namespace Nohros.Ruby.Logging.Data
{
  public interface IServiceRepositoryFactory
  {
    /// <summary>
    /// Creates a new instance of the <see cref="IServiceRepository"/> class
    /// using the given provider options.
    /// </summary>
    /// <param name="options">
    /// A <see cref="IDictionary{TKey,TValue}"/> containing the provider
    /// options.
    /// </param>
    /// <returns>
    /// The newly created <see cref="IServiceRepository"/> object.
    /// </returns>
    IServiceRepository CreateServiceRepository(
      IDictionary<string, string> options);
  }
}
