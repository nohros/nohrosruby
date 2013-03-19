using System;
using Nohros.Configuration;

namespace Nohros.Ruby
{
  public interface ITrackerSettings : IConfiguration
  {
    int BroadcastPort { get; }
  }
}
