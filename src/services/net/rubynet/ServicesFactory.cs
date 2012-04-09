using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nohros.Ruby.Service.Net
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
    public ServicesFactory() { }
    #endregion

    /// <summary>
    /// Creates an instance of the service related with the specifd
    /// service factory.
    /// </summary>
    /// <param name="service_factory_class_type">The <see cref="Type"/>
    /// of the service factory that is used to creates a instance of
    /// the service.</param>
    /// <param name="service_command_line">The command line to pass
    /// to the service.</param>
    /// <returns>
    /// A instance of the <see cref="IRubyService"/> class related with the
    /// specified service factory type.
    /// </returns>
    /// <remarks>
    /// This method always returns a valid instance of the
    /// <see cref="IRubyService"/> class or throws an exception.
    /// </remarks>
    /// <exception cref="TypeLoadException">An instance of the
    /// factory could not be created or the factory fails to create
    /// a instance of the service.</exception>
    public IRubyService CreateService(Type service_factory_class_type,
      string service_command_line) {
      // checks if the service type is assignable from  the IRubyService
      // interface.
        if (!typeof(IRubyServiceFactory).IsAssignableFrom(
          service_factory_class_type))
        throw new TypeLoadException(
          string.Format(
            Resources.log_irubyservice_load_error,
            service_factory_class_type.Name));

      // attempt to instantiate the client service factory
      IRubyServiceFactory service_factory =
        Activator.CreateInstance(
          service_factory_class_type) as IRubyServiceFactory;

      if (service_factory == null)
        throw new ArgumentException(
          string.Format(
            Resources.log_irubyservice_factory_constructor_error,
            service_factory));

      // instantiate a new service object using the service factory object.
      // A try...catch block is used here to pack any exception raised
      // by the factory into the defined logger format.
      IRubyService service;
      try {
        service = service_factory.CreateService(service_command_line);
      } catch(Exception exception) {
        throw new TypeLoadException(string.Format(
          Resources.log_irubyservice_load_error,
          service_factory_class_type.FullName
          ), exception);
      }

      if (service == null) {
        throw new TypeLoadException(string.Format(
          Resources.log_irubyservice_load_error,
          service_factory_class_type.FullName
          ));
      }
      return service;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="IRubyServiceHost"/> class
    /// by using the specified service and IPC channel name.
    /// </summary>
    /// <param name="service">The service to host</param>
    /// <param name="pipe_channel_name">The name of the IPC channel used
    /// for server/client communication.</param>
    /// <returns></returns>
    public RubyServiceHost CreateServiceHost(IRubyService service,
      string pipe_channel_name) {
      RubyServiceHost host = new RubyServiceHost(service, pipe_channel_name);
      return host;
    }
  }
}
