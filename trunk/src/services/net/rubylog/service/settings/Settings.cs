using System;

namespace Nohros.Ruby.Logging
{
  /// <summary>
  /// An implemetation of all the application settings.
  /// </summary>
  /// <seealso cref="IAggregatorSettings"/>
  public partial class Settings : Configuration.Configuration,
                                  IAggregatorSettings
  {
    readonly int publisher_port_;
    IAggregatorDataProvider aggregator_data_provider_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="Settings"/>
    /// class that initializes all members to its default values.
    /// </summary>
    public Settings(Builder builder) : base(builder) {
      publisher_port_ = builder.PublisherPort;
    }
    #endregion

    /// <inheritdoc/>
    public int PublisherPort {
      get { return publisher_port_; }
    }
  }
}
