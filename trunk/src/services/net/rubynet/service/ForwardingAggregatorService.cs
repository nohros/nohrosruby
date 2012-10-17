using System;
using Nohros.Ruby.Logging;

namespace Nohros.Ruby
{
  /// <summary>
  /// A implementation of the <see cref="IAggregatorService"/> that forwards
  /// its <see cref="Log"/> method to another <see cref="IAggregatorService"/>.
  /// </summary>
  public class ForwardingAggregatorService : IAggregatorService
  {
    IAggregatorService backing_aggregator_service_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="ForwardingAggregatorService"/>
    /// class using the specified <see cref="IAggregatorService"/> as backing
    /// aggregator.
    /// </summary>
    /// <param name="aggregator_service">
    /// A <see cref="IAggregatorService"/> to forwards the <see cref="Log"/>
    /// method.
    /// </param>
    public ForwardingAggregatorService(IAggregatorService aggregator_service) {
      backing_aggregator_service_ = aggregator_service;
    }
    #endregion

    /// <inheritdoc/>
    public void Log(LogMessage log) {
      backing_aggregator_service_.Log(log);
    }

    /// <inheritdoc/>
    public IAggregatorService BackingAggregatorService {
      get { return backing_aggregator_service_; }
      set { backing_aggregator_service_ = value; }
    }
  }
}
