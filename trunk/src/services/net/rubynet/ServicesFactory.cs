using System;
using System.Collections.Generic;
using Nohros.Configuration;
using Nohros.Providers;
using Nohros.Ruby.Protocol.Control;

namespace Nohros.Ruby
{
  /// <summary>
  /// A factory used to build instance of the classes related to services.
  /// </summary>
  internal class ServicesFactory
  {
    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="ServicesFactory"/> class.
    /// </summary>
    public ServicesFactory() {
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
      ProviderNode provider =
        new ProviderNode.Builder(
          options[Strings.kServiceAssembly], options[Strings.kServiceType])
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
