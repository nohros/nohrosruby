using System;
using System.Collections.Generic;

namespace Nohros.Ruby.Data
{
  /// <summary>
  /// A factory for <see cref="IServicesRepositoryFactory"/> class.
  /// </summary>
  public interface IServicesRepositoryFactory
  {
    /// <summary>
    /// Creates a new instance of the <see cref="IServicesRepository"/> class
    /// by using the specified options.
    /// </summary>
    /// <param name="options">
    /// A collection of key value pairs containg the user defined options for
    /// the repository.
    /// </param>
    /// <returns></returns>
    IServicesRepository CreateServicesRepository(
      IDictionary<string, string> options);
  }
}
