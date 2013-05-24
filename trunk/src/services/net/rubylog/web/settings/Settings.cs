using System;

namespace Nohros.Ruby.Logging
{
  public partial class Settings : Configuration.Configuration, ISettings
  {
    #region .ctor
    public Settings(Builder builder) : base(builder) {
      PublisherEndpoint = builder.PublisherEndpoint;
    }
    #endregion

    public string PublisherEndpoint { get; private set; }
  }
}
