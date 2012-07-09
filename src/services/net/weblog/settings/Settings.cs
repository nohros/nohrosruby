using System;
using Nohros.Configuration;

namespace Nohros.Ruby.Logging
{
  /// <summary>
  /// An implemetation of all the application settings.
  /// </summary>
  /// <seealso cref="IAggregatorSettings"/>
  /// <seealso cref="ISettings"/>
  public class Settings : MustConfiguration, IAggregatorSettings, ISettings
  {
    IAggregatorDataProvider aggregator_data_provider_;
    int publisher_port_;
    int listener_port_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="Settings"/>
    /// class that initializes all members to its default values.
    /// </summary>
    public Settings() {
      publisher_port_ = 8523;
      listener_port_ = 8524;
      aggregator_data_provider_ = null;
    }
    #endregion

    /// <inheritdoc/>
    public IAggregatorDataProvider AggregatorDataProvider {
      get { return aggregator_data_provider_; }
      internal set { aggregator_data_provider_ = value; }
    }

    /// <inheritdoc/>
    public int PublisherPort {
      get { return publisher_port_; }
      internal set { publisher_port_ = value; }
    }

    /// <inheritdoc/>
    public int ListenerPort {
      get { return listener_port_; }
      internal set { listener_port_ = 0; }
    }
  }
}
