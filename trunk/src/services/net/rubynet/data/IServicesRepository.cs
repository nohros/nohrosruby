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
    /// <param name="criteria">
    /// A <see cref="ServicesByFactsCriteria"/> object containing the facts to
    /// search for.
    /// </param>
    /// <returns></returns>
    IEnumerable<ZMQEndPoint> Query(ServiceFacts criteria);

    /// <summary>
    /// Adds services to the repository.
    /// </summary>
    void Add(ServiceEndpoint criteria);

    /// <summary>
    /// Removes the service from the repository.
    /// </summary>
    /// <param name="criteria">
    /// The <see cref="ServiceFacts"/> associated with the services to be
    /// removed.
    /// </param>
    void Remove(ServiceFacts criteria);
  }
}
