using System;
using System.Collections.Generic;
using System.IO;
using Google.ProtocolBuffers;
using Nohros.Ruby.Protocol;
using Nohros.Ruby.Protocol.Control;

namespace Nohros.Ruby.Shell
{
  /// <summary>
  /// Initializes a new service instance.
  /// </summary>
  /// <remarks>
  /// The main reasons that causes a service to not starts are:
  /// <list type="bullet">
  /// <item>
  /// The specified assembly does not exists physically.
  /// </item>
  /// <item>
  /// The specified type could not be loaded.
  /// </item>
  /// <item>
  /// The specified class type could does not implements the
  /// <see cref="IRubyService"/> interface.</item>
  /// </list>
  /// </remarks>
  internal class StartCommand : ShellCommand
  {
    const string kServiceDirectory = "service-directory";

    readonly IRubySettings settings_;
    readonly CommandLine switches_;
    readonly IRubyLogger logger_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="StartCommand"/> by using
    /// the specified command line switches.
    /// </summary>
    public StartCommand(CommandLine switches, IRubySettings settings)
      : base(ShellStrings.kStartCommand) {
      switches_ = switches;
      settings_ = settings;
      logger_ = RubyLogger.ForCurrentProcess;
    }
    #endregion

    /// <summary>
    /// Host and runs a service.
    /// </summary>
    /// <remarks>
    /// If the hosted service throws an exception, it will be propagated to the
    /// caller.
    /// </remarks>
    public override void Run(ShellRubyProcess process) {
      if (logger_.IsDebugEnabled) {
        logger_.Debug("StartCommand/Run");
      }

      string service_assembly_location = GetServiceLocation();
      string service_factory_type_name = GetServiceFactoryTypeName();
      string service_switches = GetServiceSwicthes(service_factory_type_name);

      ServiceControlMessage start_control_message =
        new ServiceControlMessage.Builder()
          .SetType(ServiceControlMessageType.kServiceControlStart)
          .AddArguments(
            new KeyValuePair.Builder()
              .SetKey(Strings.kServiceAssembly)
              .SetValue(service_assembly_location)
              .Build())
          .AddArguments(
            new KeyValuePair.Builder()
              .SetKey(Strings.kServiceType)
              .SetValue(service_factory_type_name + "," +
                Path.GetFileNameWithoutExtension(service_assembly_location))
              .Build())
          .AddArguments(
            new KeyValuePair.Builder()
              .SetKey(Strings.kServiceSwitches)
              .SetValue(service_switches)
              .Build())
          .Build();

      process.OnMessagePacketReceived(
        GetRubyMessagePacketHeader(start_control_message));
    }

    RubyMessagePacket GetRubyMessagePacketHeader(
      ServiceControlMessage start_control_message) {
      ByteString start_control_message_bytes =
        start_control_message.ToByteString();
      RubyMessageHeader header = new RubyMessageHeader.Builder()
        .SetId(0)
        .AddFacts(
          new KeyValuePair.Builder()
            .SetKey(StringResources.kServiceNameFact)
            .SetValue(Strings.kServiceHostServiceName)
            .Build())
        .SetSize(start_control_message_bytes.Length)
        .Build();

      RubyMessage message = new RubyMessage.Builder()
        .SetId(0)
        .SetToken("node-service-control")
        .SetType((int) NodeMessageType.kServiceControl)
        .SetMessage(start_control_message_bytes)
        .Build();

      return new RubyMessagePacket.Builder()
        .SetHeader(header)
        .SetHeaderSize(header.SerializedSize)
        .SetMessage(message)
        .SetSize(header.SerializedSize + 2 + message.SerializedSize)
        .Build();
    }

    string GetServiceFactoryTypeName() {
      string service_factory_type_name =
        switches_.GetSwitchValue(Strings.kServiceType);
      if (service_factory_type_name == string.Empty) {
        throw new ArgumentException(string.Format(
          Resources.Arg_MissingOrInvalid, Strings.kServiceType));
      }
      return service_factory_type_name;
    }

    /// <summary>
    /// Builds a command line to pass to the assembly. The command line
    /// program argument will be set to the class_type_name. So, the
    /// service could use a single instance of the service factory to create
    /// all the services that it implements.
    /// </summary>
    string GetServiceSwicthes(string program) {
      string service_switches = program;
      IList<string> loose_values = switches_.LooseValues;
      for (int i = 0, j = loose_values.Count; i < j; i++) {
        service_switches += string.Concat(" ", loose_values[i]);
      }
      return service_switches;
    }

    string GetServiceLocation() {
      string service_assembly_location =
        switches_.GetSwitchValue(Strings.kServiceAssembly);

      // If the service name was not specified, use the assembly as the
      // service name.
      string service_directory = switches_.GetSwitchValue(kServiceDirectory);
      if (service_directory == string.Empty) {
        service_directory =
          Path.GetFileNameWithoutExtension(service_assembly_location);
      }

      // If the path is not absolute it must be relative to the services
      // directory
      if (!Path.IsPathRooted(service_assembly_location)) {
        service_assembly_location =
          Path.Combine(
            settings_.NodeDirectory,
            settings_.ServicesFolder,
            service_directory,
            service_assembly_location);
      }

      if (!File.Exists(service_assembly_location)) {
        throw new ArgumentException(
          string.Format(Resources.log_assembly_not_found,
            service_assembly_location));
      }
      return service_assembly_location;
    }
  }
}
