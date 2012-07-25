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
    int self_host_port_;
    bool self_host_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="Settings"/>
    /// class that initializes all members to its default values.
    /// </summary>
    public Settings() {
      publisher_port_ = 8523;
      self_host_ = false;
      self_host_port_ = 8520;
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
    public bool SelfHost {
      get { return self_host_; }
      internal set { self_host_ = value; }
    }

    /// <inheritdoc/>
    public int SelfHostPort {
      get { return self_host_port_; }
      internal set { self_host_port_ = value; }
    }
  }
}
