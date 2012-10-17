using System;
using Nohros.Ruby.Logging;

namespace Nohros.Ruby
{
  /// <summary>
  /// A implementation of the <see cref="IAggregatorService"/> that do no
  /// operation.
  /// </summary>
  public class NopAggregatorService : IAggregatorService
  {
    public void Log(LogMessage log) {
    }
  }
}
