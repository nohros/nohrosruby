using System;
using System.Collections.Generic;
using System.Threading;

using Nohros.Resources;

namespace Nohros.Ruby
{
  /// <summary>
  /// Provides a skeletal implementation of the <see cref="IRubyProcess"/>
  /// interface to reduce the effort required to implement it.
  /// </summary>
  internal abstract class AbstractRubyProcess : IRubyProcess
  {
    const string kClassName = "Nohros.Ruby.AbstractRubyProcess";

    const int kMaxRunningServices = 10;

    readonly IRubyMessageChannel ruby_message_channel_;
    readonly Dictionary<string, RubyServiceHost> hosted_services_;
    readonly object hosted_service_mutex_;

    readonly IRubyLogger logger = RubyLogger.ForCurrentProcess;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="AbstractRubyProcess"/>
    /// class by using the specified <see cref="RubyMessageChannel"/> object.
    /// </summary>
    /// <param name="ruby_message_channel">
    /// A <see cref="RubyMessageChannel"/> object that is used to handle the
    /// communication with the ruby service node.
    /// </param>
    protected AbstractRubyProcess(IRubyMessageChannel ruby_message_channel) {
      ruby_message_channel_ = ruby_message_channel;
      hosted_services_ = new Dictionary<string, RubyServiceHost>();
      hosted_service_mutex_ = new object();
    }
    #endregion

    /// <inheritdoc/>
    public virtual void Run() {
      Run(string.Empty);
    }

    /// <inheritdoc/>
    public abstract void Run(string command_line_string);

    /// <summary>
    /// Hosts a <see cref="IRubyService"/> implementation as represented by
    /// <paramref name="service"/> in the running process.
    /// </summary>
    /// <param name="service">
    /// The service to host.
    /// </param>
    /// <returns>
    /// <c>true</c> if the service was successfully hosted; otherwise,
    /// <c>false</c>.
    /// </returns>
    /// <remarks>
    /// The service runs in a sandbox that guarantee that any exceptions that
    /// occurs on the service is not bubbled up to the application. This
    /// assure that one service does not affect the execution of the others.
    /// <para>
    /// Each hosted service runs in into a dedicated thread. The thread will be
    /// a background thread, so we can force it to shut down without hanging
    /// the application.
    /// </para>
    /// <para>
    /// A service fails to be hosted if the maximum number of running service
    /// has been reached.
    /// </para>
    /// </remarks>
    public bool HostService(IRubyService service) {
      if (hosted_services_.Count > kMaxRunningServices) {
        logger.Warn(
          "The limit of simultaneous running services has been reached");
        return false;
      }

      RubyServiceHost host = new RubyServiceHost(service, ruby_message_channel_);
      if (hosted_services_.ContainsKey(service.Name)) {
        logger.Warn("The service " + service.Name + " is already running");
        return false;
      }

      // Keep hosted_services_ thread safe, since it is manipulated by more
      // than one thread (This thread and the thread that is running the
      // service).
      lock (hosted_service_mutex_) {
        hosted_services_.Add(service.Name, host);
      }

      // A try/catch block is used here to ensure the consistence of the
      // list of running services.
      try {
        Thread thread = new Thread(ThreadMain);
        thread.IsBackground = true;
        thread.Start(host);
        return true;
      } catch (Exception exception) {
        lock (hosted_service_mutex_) {
          hosted_services_.Remove(service.Name);
        }        
        logger.Error(string.Format(StringResources.Log_MethodThrowsException,
          kClassName, "HostService"), exception);
        return false;
      }
    }

    void ThreadMain(object o) {
      RubyServiceHost host = o as RubyServiceHost;
#if DEBUG
      if (host == null) {
        throw new ArgumentException(
          "object 'o' is not an instance of the RubyServiceHost class");
      }
#endif
      // A try/catch block is used here to ensure the consistence of the
      // list of running services and to isolate one service from another.
      try {
        host.Start();
      } catch(Exception exception) {
        logger.Error(string.Format(StringResources.Log_MethodThrowsException,
          kClassName, "ThreadMain"));
      }

      lock(hosted_service_mutex_) {
        hosted_services_.Remove(host.Service.Name);
      }
    }

    /// <inheritdoc/>
    public virtual IRubyMessageChannel RubyMessageChannel {
      get { return ruby_message_channel_; }
    }
  }
}
