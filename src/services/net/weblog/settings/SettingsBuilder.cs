using System;
using Nohros.Configuration.Builders;

namespace Nohros.Ruby.Logging
{
  public partial class Settings
  {
    public class Builder : AbstractConfigurationBuilder<Settings>
    {
      IAggregatorDataProvider aggregator_data_provider_;
      int publisher_port_;

      #region .ctor
      /// <summary>
      /// Initializes a new instance of the <see cref="Builder"/> class.
      /// </summary>
      public Builder() {
        publisher_port_ = 8523;
        aggregator_data_provider_ = new LoggerAggregatorDataProvider();
      }
      #endregion

      public Builder SetPublisherPort(int publisher_port) {
        publisher_port_ = publisher_port;
        return this;
      }

      public override Settings Build() {
        return new Settings(this);
      }

      public Builder SetAggregatorDataProvider(
        IAggregatorDataProvider aggregator_data_provider) {
        aggregator_data_provider_ = aggregator_data_provider;
        return this;
      }

      public IAggregatorDataProvider AggregatorDataProvider {
        get { return aggregator_data_provider_; }
      }

      public int PublisherPort {
        get { return publisher_port_; }
      }
    }
  }
}
