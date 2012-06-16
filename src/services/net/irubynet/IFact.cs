using System;

namespace Nohros.Ruby
{
  /// <summary>
  /// A <see cref="IFact"/> is a discrete bit of information about a service
  /// such as an alis or an specific configuration value.
  /// </summary>
  public interface IFact
  {
    /// <summary>
    /// Gets a string that uniquely identifies the fact within a service.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the value associated with the fact.
    /// </summary>
    string Value { get; }
  }
}
