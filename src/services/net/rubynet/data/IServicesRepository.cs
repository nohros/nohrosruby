using System;
using System.Collections.Generic;

namespace Nohros.Ruby
{
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
    IEnumerable<ZMQEndPoint> Query(ServicesByFactsCriteria criteria);
  }
}
