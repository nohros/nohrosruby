using System;

namespace Nohros.Ruby.Service.Net
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
    readonly string service_command_line_;
    readonly Type service_factory_class_type_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="StartCommand"/> by using
    /// the specified service factory type.
    /// </summary>
    /// <param name="service_factory_class_type">
    /// The assembly's fully qualified name of the class that is used to
    /// instantiate a new services.
    /// </param>
    /// <remarks>
    /// The <paramref name="service_factory_class_type"/> must implements
    /// the <see cref="IRubyServiceFactory"/> interface.
    /// </remarks>
    public StartCommand(Type service_factory_class_type)
      : base(ShellSwitches.kStartCommand) {
#if DEBUG
        if (service_factory_class_type == null)
          throw new ArgumentNullException("service_factory_class_type");
#endif
      service_factory_class_type_ = service_factory_class_type;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StartCommand"/> by using
    /// the specified service factory type.
    /// </summary>
    /// <param name="service_factory_class_type">
    /// The assembly's fully qualified name of the class that is used to
    /// instantiate a new services.
    /// </param>
    /// <param name="service_command_line">
    /// The command line string to pass to the service.
    /// </param>
    /// <remarks>
    /// The <paramref name="service_factory_class_type"/> must implements
    /// the <see cref="IRubyServiceFactory"/> interface.
    /// </remarks>
    public StartCommand(Type service_factory_class_type, string service_command_line)
      : base(ShellSwitches.kStartCommand) {

#if DEBUG
        if (service_factory_class_type == null ||
          service_command_line == null) {
          throw new ArgumentNullException(
            (service_factory_class_type == null)
              ? "service_factory_class_type" : "service_command_line");
        }
#endif
      service_factory_class_type_ = service_factory_class_type;
      service_command_line_ = service_command_line;
    }
    #endregion

    /// <summary>
    /// Host and runs a service.
    /// </summary>
    /// <remarks>
    /// If the hosted service throws an exception, it will be propagated to the
    /// caller.
    /// </remarks>
    public override void Run(RubyShell shell) {
      ServicesFactory factory = new ServicesFactory();
      IRubyService service = factory.CreateService(
        service_factory_class_type_, service_command_line_);

      RubyServiceHost host =
        factory.CreateServiceHost(service, shell.IPCChannel);

      shell.ServiceHosts.HostService(host);
    }
  }
}