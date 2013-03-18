using System;
using System.Diagnostics;
using System.Net.Sockets;
using Nohros.Configuration;
using Nohros.Extensions;
using Nohros.IO;
using Nohros.Ruby.Data;
using ZmqContext = ZMQ.Context;

namespace Nohros.Ruby
{
  public class TrackerServiceFactory : IRubyServiceFactory
  {
    public IRubyService CreateService(string command_line_string) {
      CommandLine switches = CommandLine.FromString(command_line_string);
      if (switches.HasSwitch("debug")) {
        Debugger.Launch();
      }

      string config_file_name =
        switches.GetSwitchValue(Strings.kConfigFileNameSwitch,
          Strings.kDefaultConfigFileName);
      string config_root_node =
        switches.GetSwitchValue(Strings.kConfigRootNodeSwitch,
          Strings.kDefaultConfigRootNode);
      var settings = new TrackerSettings.Loader()
        .Load(Path.AbsoluteForCallingAssembly(config_file_name),
          config_root_node);

      IServicesRepository repository = CreateServicesRepository(settings);
      var udp_client = new UdpClient();
      var broadcaster = new Broadcaster(udp_client) {
        BroadcastPort = settings.BroadcastPort
      };
      var context = new ZmqContext();
      var tracker_factory = new TrackerFactory(context);
      var service = new TrackerService(repository, broadcaster, tracker_factory);
      return service;
    }

    IServicesRepository CreateServicesRepository(ITrackerSettings settings) {
      IProviderNode provider = settings.Providers
        .GetProviderNode(Strings.kServicesRepositoryNodeName);
      return
        RuntimeTypeFactory<IServicesRepositoryFactory>
          .CreateInstanceFallback(provider, settings)
          .CreateServicesRepository(provider.Options.ToDictionary());
    }
  }
}
