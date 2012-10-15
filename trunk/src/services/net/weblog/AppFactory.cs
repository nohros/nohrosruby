using System;

namespace Nohros.Ruby.Logging
{
  public sealed class AppFactory
  {
    readonly ISettings settings_;

    #region .ctor
    public AppFactory(ISettings settings) {
      settings_ = settings;
    }
    #endregion
  }
}
