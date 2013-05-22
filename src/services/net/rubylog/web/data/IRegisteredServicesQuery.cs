using System;
using System.Collections.Generic;

namespace Nohros.Ruby.Logging.Data
{
  /// <summary>
  /// Query for all registered services.
  /// </summary>
  public interface IRegisteredServicesQuery
  {
    /// <summary>
    /// Executes the <see cref="IRegisteredServicesQuery"/> query.
    /// </summary>
    /// <returns>
    /// A list of all registered services.
    /// </returns>
    IEnumerable<Service> Execute();
  }
}