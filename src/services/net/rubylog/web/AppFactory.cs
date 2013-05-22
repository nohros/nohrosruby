using System;
using System.Configuration;
using Nohros.Configuration;
using Nohros.IO;
using Nohros.Extensions;
using ServiceStack.WebHost.Endpoints;
using ZmqContext = ZMQ.Context;

namespace Nohros.Ruby.Logging
{
  public class AppFactory
  {
    public App CreateApp(StatusManager manager, Settings settings) {
      var publisher = new HttpPublisher();
      var context = new ZmqContext();
      if (settings.PublisherEndpoint.IsNullOrEmpty()) {
        throw new ConfigurationErrorsException(
          Resources.Configuration_InvalidPublisherEndpoint.Fmt(
            settings.PublisherEndpoint));
      }
      var channel = new MessageChannel(context, settings.PublisherEndpoint);
      return new App(publisher, channel, manager, settings);
    }

    public StatusManager CreateStatusManager() {
      return new StatusManager();
    }

    public Settings CreateSettings() {
      string config_file_name =
        ConfigurationManager.AppSettings[Strings.kConfigFileNameKey];
      if (string.IsNullOrEmpty(config_file_name)) {
        config_file_name = Strings.kDefaultConfigFileName;
      }
      string config_root_node_name =
        ConfigurationManager.AppSettings[Strings.kConfigRootNodeName];
      if (string.IsNullOrEmpty(config_root_node_name)) {
        config_root_node_name = Strings.kDefaultConfigRootNode;
      }
      string config_file_path = Path.AbsoluteForApplication(config_file_name);
      return new Settings.Loader()
        .Load(config_file_path, config_root_node_name);
    }
  }
}
