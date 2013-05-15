using System;
using Nohros.Configuration.Builders;

namespace Nohros.Ruby.Logging
{
  public partial class Settings
  {
    public class Builder : AbstractConfigurationBuilder<Settings>
    {
      public string PublisherEndpoint { get; private set; }

      public Builder SetPublisherEndpoint(string publisher_endpoint) {
        PublisherEndpoint = publisher_endpoint;
        return this;
      }

      public override Settings Build() {
        return new Settings(this);
      }
    }
  }
}
