using System;
using System.Collections.Generic;

namespace Nohros.Ruby.Logging
{
  /// <summary>
  /// A factory used to create instances of the
  /// <see cref="ILogMessageRepository"/> object.
  /// </summary>
  /// <remarks>
  /// This interface implies a constructor that receives an instance of the
  /// <see cref="IAggregatorSettings"/> class or a constructor that receives no
  /// parameters. We try to build an instance if the
  /// <see cref="ILogMessageRepositoryFactory"/> class using the constructor
  /// that receives a <see cref="IAggregatorSettings"/> object and, if it fails,
  /// the constructor that receives no parameters.
  /// </remarks>
  public interface ILogMessageRepositoryFactory
  {
    /// <summary>
    /// Creates an instance of the <see cref="ILogMessageRepository"/>
    /// object.
    /// </summary>
    /// <returns></returns>
    ILogMessageRepository CreateAggregatorDataProvider(
      IDictionary<string, string> options);
  }
}
