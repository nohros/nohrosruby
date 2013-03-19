using System;
using System.Collections.Generic;

namespace Nohros.Ruby.Data
{
  /// <summary>
  /// Manages and provides information about the list of current running
  /// services.
  /// </summary>
  public interface IServicesRepository
  {
    /// <summary>
    /// Searchs for a service that matches the specified facts.
    /// </summary>
    IServicesByFactsQuery Query(out IServicesByFactsQuery query);

    /// <summary>
    /// Adds services to the repository.
    /// </summary>
    INewServiceCommand Query(out INewServiceCommand command);

    /// <summary>
    /// Removes the service from the repository.
    /// </summary>
    IRemoveServicesCommand Query(out IRemoveServicesCommand query);
  }
}
