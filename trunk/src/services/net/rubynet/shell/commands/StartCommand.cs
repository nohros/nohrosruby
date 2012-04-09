using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;

using Nohros.Desktop;

namespace Nohros.Ruby.Service.Net
{
  /// <summary>
  /// Initializes a new service instance.
  /// </summary>
  /// <remarks>
  /// The main reasons that causes a service to not starts are:
  /// <list type="bullet">
  /// <item>The specified assembly does not exists physically.</item>
  /// <item>The specified type could not be loaded.</item>
  /// <item>The specified class type could does not implements the
  /// <see cref="IRubyService"/> interface.</item>
  /// </list>
  /// </remarks>
  internal class StartCommand : ShellCommand
  {
    string pipe_channel_name_;
    string service_command_line_;
    Type service_factory_class_type_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="StartCommand"/> by using
    /// the specified service factory type.
    /// </summary>
    /// <param name="service_factory_class_type">The assembly's fully
    /// qualified name of the class that is used to instantiate a
    /// new services.</param>
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
    /// <param name="service_factory_class_type">The assembly's fully
    /// qualified name of the class that is used to instantiate a
    /// new services.</param>
    /// <param name="service_command_line">The command line string to pass
    /// to the service.</param>
    /// <remarks>
    /// The <paramref name="service_factory_class_type"/> must implements
    /// the <see cref="IRubyServiceFactory"/> interface.
    /// </remarks>
    public StartCommand(Type service_factory_class_type,
      string service_command_line)
        : base(ShellSwitches.kStartCommand) {
#if DEBUG
        if (service_factory_class_type == null ||
          service_command_line == null) {
            throw new ArgumentNullException(
              (service_factory_class_type == null)
                ? "service_factory_class_type" : "service_command_line");
        }
#endif
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StartCommand"/> by using
    /// the specified service factory type, IPC channel name and command line
    /// string.
    /// </summary>
    /// <param name="service_class_type">The .NET class type of the service.
    /// This class must implements the <see cref="IRubyService"/> interface.
    /// </param>
    /// <param name="pipe_channel_name">The name of the IPC channel that will
    /// be used to handle the server communication. If this value is null,
    /// the IPC channel will not be created.</param>
    /// <param name="service_command_line">A string that is passed to the
    /// hosted service and acts as the service command line.</param>
    /// <exception cref="ArgumentNullException">
    /// One of the argument is a null reference.</exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="service_class_type"/> does not implements the
    /// <see cref="IRubyService"/> interface.</exception>
    public StartCommand(Type service_factory_class_type,
      string pipe_channel_name, string service_command_line)
      : base(ShellSwitches.kStartCommand) {

#if DEBUG
        if (service_factory_class_type == null
          || pipe_channel_name == null
          || service_command_line == null)
          throw new ArgumentNullException((service_factory_class_type == null)
            ? "service_class_type" : ((pipe_channel_name == null)
            ? "pipe_channel_name" : "service_command_line"));
#endif

      service_factory_class_type_ = service_factory_class_type;
      pipe_channel_name_ = pipe_channel_name;
      service_command_line_ = (service_command_line == null)
        ? string.Empty : service_command_line;
    }
    #endregion

    /// <summary>
    /// Host and runs a service.
    /// </summary>
    /// <returns>An <see cref="IRubyServiceHost"/> object repreenting the
    /// running service.</returns>
    /// <remarks>
    /// If the hosted service throws an exception, it will be propagated to
    /// the caller.
    /// </remarks>
    /*[Obsolete("This method is obsolete. Use the StartService(bool) method passing false as parameter.")]
    public IRubyServiceHost StartService() {
      // TODO: get the default logger using the configuration file.
      return StartService(false);
    }*/

    /// <summary>
    /// Host and runs a service.
    /// </summary>
    /// <param name="delay_start">true to not start the service and transfer this responsability to the caller;
    /// otherwise, false.</param>
    /// <param name="service_logger">An <see cref="ILog"/> object that can be used by the service
    /// to log its messages.</param>
    /// <returns>An <see cref="IRubyServiceHost"/> object repreenting the running service.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="service_logger_"/>is a null reference.</exception>
    /// <remarks>
    /// If the hosted service throws an exception, it will be propagated to the caller.
    /// <para>
    /// If the parameter <paramref name="delay_start"/> is true the service will not be started, the caller
    /// should be call the <see cref="IRubyServiceHost.StartService()"/>method of the returned object in order to
    /// starts the service.
    /// </para>
    /// </remarks>
    public override void Run(RubyShell shell) {
      ServicesFactory factory = new ServicesFactory();
      IRubyService service = factory.CreateService(
        service_factory_class_type_, service_command_line_);

      RubyServiceHost host =
        factory.CreateServiceHost(service, pipe_channel_name_);

      shell.ServiceHosts.HostService(host);
    }
  }
}