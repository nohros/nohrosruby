using System;
using System.Collections.Generic;
using System.IO;
using Nohros.Configuration;
using Nohros.Providers;
using Nohros.Ruby.Protocol;
using Nohros.Ruby.Protocol.Control;

namespace Nohros.Ruby
{
  /// <summary>
  /// A factory used to build instance of the classes related to services.
  /// </summary>
  internal class ServicesFactory
  {
    readonly IRubySettings settings_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="ServicesFactory"/> class.
    /// </summary>
    public ServicesFactory(IRubySettings settings) {
      settings_ = settings;
    }
    #endregion

    public IRubyService CreateService(ServiceControlMessage message) {
      IDictionary<string, string> options = GetServiceOptions(message);
      IRubyServiceFactory factory = GetServiceFactory(options);
      string service_switches = ProviderOptions.GetIfExists(options,
        Strings.kServiceSwitches, string.Empty);
      return factory.CreateService(service_switches);
    }

    IRubyServiceFactory GetServiceFactory(IDictionary<string, string> options) {
      string service_factory_assembly_location =
        options[Strings.kServiceAssembly];
      string service_factory_class_type = options[Strings.kServiceType];

      // If the assembly path is relative, we need to resolve it using the
      // configured services directory as base path.
      if (!Path.IsPathRooted(service_factory_assembly_location)) {
        service_factory_assembly_location =
          Path.GetFullPath(Path.Combine(settings_.NodeDirectory,
            settings_.ServicesFolder, service_factory_assembly_location));
      }
      string service_factory_assembly =
        Path.GetFileName(service_factory_assembly_location);
      service_factory_assembly_location =
        Path.GetDirectoryName(service_factory_assembly_location);
      ProviderNode provider =
        new ProviderNode.Builder(
          service_factory_assembly, service_factory_class_type)
          .SetLocation(service_factory_assembly_location)
          .SetOptions(options)
          .Build();
      return ProviderFactory<IRubyServiceFactory>.CreateProviderFactory(provider);
    }

    IDictionary<string, string> GetServiceOptions(ServiceControlMessage message) {
      IList<KeyValuePair> arguments = message.ArgumentsList;
      Dictionary<string, string> options =
        new Dictionary<string, string>(arguments.Count);
      for (int i = 0, j = arguments.Count; i < j; i++) {
        KeyValuePair argument = arguments[i];
        options.Add(argument.Key, argument.Value);
      }
      return options;
    }
  }
}
