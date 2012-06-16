using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Nohros.Ruby
{
  /// <summary>
  /// A collection of <see cref="RubyServiceHost"/> objects.
  /// </summary>
  internal class RubyServiceHosts: IEnumerable<RubyServiceHost>, IEnumerable
  {
    const int kMaxThreads = 10;

    Dictionary<string, RubyServiceHost> hosts_;
    Mutex host_dictionary_mutex_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="RubyServiceHosts"/>.
    /// </summary>
    public RubyServiceHosts() : this(0) { }

    /// <summary>
    /// Initializes a new instabnce of the <see cref="RubyServiceHosts"/> by
    /// using the specified initial capacity.
    /// </summary>
    /// <param name="capacity"></param>
    public RubyServiceHosts(int capacity) {
      hosts_ = new Dictionary<string, RubyServiceHost>(capacity);
      host_dictionary_mutex_ = new Mutex();
    }
    #endregion

    /// <summary>
    /// Gets the <see cref="RubyServiceHost"/> object associated with the
    /// specified service name.
    /// </summary>
    /// <param name="service_name">The name of the associated service of
    /// the host to get.</param>
    /// <param name="host">When this method returns, contains the
    /// <see cref="RubyServiceHost"/> bject associated with the specified
    /// service name, if the service name is found; otherwise, null. This
    /// parameter is passed uninitialized.</param>
    /// <returns></returns>
    public bool TryGetHost(string service_name,
      out RubyServiceHost host) {
      return hosts_.TryGetValue(service_name, out host);
    }

    /// <summary>
    /// Creates a dedicated service host to run the specified
    /// <see cref="IRubyService"/>.
    /// </summary>
    /// <param name="service">The service to host.</param>
    /// <remarks>
    /// The created <see cref="IRubyServiceHost"/> object guarantee that any
    /// exception that occurs on the service is not bubbled up to the
    /// application. This provides a sandbox mechanisn for the services and
    /// assure that one service does not affect the execution of the others.
    /// <para>
    /// Each <see cref="IRubyServiceHost"/> is executed into a dedicated
    /// thread. The thread will be a background thread, so we can force the
    /// it to shuts down without hangs the application.
    /// </para>
    /// </remarks>
    public void HostService(RubyServiceHost service_host) {
      if (hosts_.Count > kMaxThreads) {
        RubyLogger.ForCurrentProcess.Warn(
          Resources.log_max_number_services);
        return;
      }

      // create the Thread that will run the service code
      ParameterizedThreadStart parameterized_thread_start = ServiceThreadRoutine;

      Thread service_thread = new Thread(ServiceThreadRoutine);
      service_thread.IsBackground = true;

      // start the service
      service_thread.Start(service_host);
    }

    /// <summary>
    /// Stops a running service and remove it from the list of running
    /// services.
    /// </summary>
    /// <param name="service_name">The name of the service to stop.</param>
    public bool UnhostService(string service_name) {
      IRubyLogger logger = RubyLogger.ForCurrentProcess;
      bool service_is_running = false;

      host_dictionary_mutex_.WaitOne();

      RubyServiceHost host;
      service_is_running = hosts_.TryGetValue(service_name, out host);
      if (!service_is_running && logger.IsDebugEnabled) {
        logger.Debug(string.Concat(
          "[Nohros.Ruby.RubyServiceHosts   HostService]",
          string.Format(
            Resources.log_service_not_running, service_name)));
      }

      hosts_.Remove(service_name);
      host_dictionary_mutex_.ReleaseMutex();

      return service_is_running;
    }

    /// <summary>
    /// The method to be executed by the background threads.
    /// </summary>
    /// <remarks>
    /// This method should runs into a dedicated thread and after it returns
    /// the thread will be terminated and removed from the list of running
    /// threads.
    /// </remarks>
    void ServiceThreadRoutine(object param) {
      RubyServiceHost service_host = (RubyServiceHost)param;

      string service_name = service_host.Service.Name;

      IRubyLogger logger = RubyLogger.ForCurrentProcess;

      // Accordinly to the MSDN, there is no such thing as an unhandled
      // exception on a thread created with the |Run| method of the
      // |Thread| class. When code running on such a thread throws an
      // exception that it does not handle, the runtime prints the
      // exception stack trace to the console and then gracefully
      // terminates the thread. So we need to handle the excetpions to
      // avoid keep an invalid thread into our list of running services
      try {
        // we need to add the service thread to the list of running services
        // before the service starts, because the service blocks the current
        // thread until it finish you work.
        host_dictionary_mutex_.WaitOne();
        hosts_.Add(service_name, service_host);
        host_dictionary_mutex_.ReleaseMutex();

        #region debugging
        if (logger.IsDebugEnabled)
          logger.Debug("[StartService   Nohros.Ruby]   The service " + service_name + " has been started.");
        #endregion

        service_host.Start();
      } catch (Exception e) {
        logger.Error("[StartService   Nohros.Ruby]", e);
      }

      // the service has been finished your work, we can remove it from the
      // list of running services.
      hosts_.Remove(service_name);

      if (logger.IsDebugEnabled)
        logger.Debug("[StartService   Nohros.Ruby]   The service " + service_name + " has been finished.");
    }

    /// <summary>
    /// Gets or sets the <see cref="RubyServiceHost"/> associated with the
    /// specified service name.
    /// </summary>
    /// <param name="service_name">The name of the related service.</param>
    /// <returns>The <see cref="RubyServiceHost"/> associated with the
    /// specified service name. If the specified service is not found, a get
    /// operation throws a <see cref="KeyNotFoundException"/>, and a set
    /// operation creates a new elements using the specified service name
    /// as a key.</returns>
    public RubyServiceHost this[string service_name] {
      get {
        host_dictionary_mutex_.WaitOne();
        RubyServiceHost host = hosts_[service_name];
        host_dictionary_mutex_.ReleaseMutex();

        return host;
      }
    }

    #region IEnumerable
    /// <summary>
    /// Returns an enumerator that iterates through a collection.
    /// </summary>
    /// <returns>
    /// An <see cref="IEnumerator"/> object that can be used to iterate through
    /// the collection.
    /// </returns>
    /// <remarks>
    /// The returned enumerator is the same that is return by the internal
    /// collection that holds the <see cref="RubyServiceHost"/> objects.
    /// </remarks>
    IEnumerator IEnumerable.GetEnumerator() {
      ICollection<RubyServiceHost> hosts = hosts_.Values;
      foreach (RubyServiceHost host in hosts) {
        yield return host;
      }
    }

    /// <summary>
    /// Returns an enumerator that iterates through a collection.
    /// </summary>
    /// <returns>
    /// An <see cref="IEnumerator"/> object that can be used to iterate through
    /// the collection.
    /// </returns>
    public IEnumerator<RubyServiceHost> GetEnumerator() {
      return GetEnumerator();
    }
    #endregion

    /// <summary>
    /// Gets the number of service hosts contained into this collection.
    /// </summary>
    public int Count {
      get { return hosts_.Count; }
    }
  }
}