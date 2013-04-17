using System;
using Nohros.Configuration.Builders;

namespace Nohros.Ruby.Logging
{
  public partial class Settings
  {
    public class Builder : AbstractConfigurationBuilder<Settings>
    {
      #region .ctor
      /// <summary>
      /// Initializes a new instance of the <see cref="Builder"/> class.
      /// </summary>
      public Builder() {
        PublisherPort = 8523;
      }
      #endregion

      public Builder SetPublisherPort(int publisher_port) {
        PublisherPort = publisher_port;
        return this;
      }

      public override Settings Build() {
        return new Settings(this);
      }

      public int PublisherPort { get; set; }
    }
  }
}
