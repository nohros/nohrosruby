using System;

namespace Nohros.Ruby
{
  /// <summary>
  /// The tracker's repository.
  /// </summary>
  public interface ITrackerRepository
  {
    /// <summary>
    /// Register a service's metadata in the tracker repository.
    /// </summary>
    /// <param name="service">
    /// A <see cref="Fact"/> object containing the information about
    /// the service to be registered.
    /// </param>
    /// <returns>
    /// The ID of the registered service.
    /// </returns>
    int Add(Fact service);

    Fact Query(ServiceByNameCriteria service);
  }
}
