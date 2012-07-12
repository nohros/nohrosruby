using System;
using System.Collections.Generic;

namespace Nohros.Ruby.Logging
{
  /// <summary>
  /// A factory used to create instances of the
  /// <see cref="IAggregatorDataProvider"/> object.
  /// </summary>
  /// <remarks>
  /// This interface implies a constructor that receives an instance of the
  /// <see cref="IAggregatorSettings"/> class or a constructor that receives no
  /// parameters. We try to build an instance if the
  /// <see cref="IAggregatorDataProviderFactory"/> class using the constructor
  /// that receives a <see cref="IAggregatorSettings"/> object and, if it fails,
  /// the constructor that receives no parameters.
  /// </remarks>
  public interface IAggregatorDataProviderFactory
  {
    /// <summary>
    /// Creates an instance of the <see cref="IAggregatorDataProvider"/>
    /// object.
    /// </summary>
    /// <returns></returns>
    IAggregatorDataProvider CreateAggregatorDataProvider(
      IDictionary<string, string> options);
  }
}
