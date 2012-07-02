using System;
using System.Collections.Generic;
using System.Threading;
using Nohros.Concurrent;
using Nohros.Resources;
using Nohros.Ruby.Protocol;
using Nohros.Ruby.Protocol.Control;

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

    readonly object hosted_service_mutex_;
    readonly Dictionary<string, RubyServiceHost> hosted_services_;
    readonly IRubyLogger logger_;
    readonly IRubyMessageChannel ruby_message_channel_;
    readonly IRubySettings settings_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="AbstractRubyProcess"/>
    /// class by using the specified <see cref="RubyMessageChannel"/> object.
    /// </summary>
    /// <param name="ruby_message_channel">
    /// A <see cref="RubyMessageChannel"/> object that is used to handle the
    /// communication with the ruby service node.
    /// </param>
    protected AbstractRubyProcess(IRubySettings settings,
      IRubyMessageChannel ruby_message_channel) {
      ruby_message_channel_ = ruby_message_channel;
      hosted_services_ = new Dictionary<string, RubyServiceHost>();
      hosted_service_mutex_ = new object();
      logger_ = RubyLogger.ForCurrentProcess;
      settings_ = settings;
    }
    #endregion

    /// <inheritdoc/>
    public virtual void Run() {
      Run(string.Empty);
    }

    /// <inheritdoc/>
    public virtual void Run(string command_line_string) {
      RubyMessageChannel.AddListener(this, Executors.SameThreadExecutor(),
        Strings.kRubyHostServiceName);
    }

    /// <inheritdoc/>
    public void OnMessagePacketReceived(RubyMessagePacket packet) {
      try {
        ServiceControlMessage service_control_message =
          ServiceControlMessage.ParseFrom(packet.Message.Message);
        switch (packet.Message.Type) {
          case (int) ServiceControlEventType.kServiceControlEventStart:
            StartService(service_control_message);
            break;

          case (int) ServiceControlEventType.kServiceControlEventStop:
            StopService(service_control_message);
            break;
        }
      } catch (Exception exception) {
        logger_.Error(string.Format(StringResources.Log_MethodThrowsException,
          kClassName, "OnMessagePacketReceived"), exception);
      }
    }

    /// <inheritdoc/>
    public virtual IRubyMessageChannel RubyMessageChannel {
      get { return ruby_message_channel_; }
    }

    /// <summary>
    /// Hosts a <see cref="IRubyService"/> implementation in the running
    /// process.
    /// </summary>
    /// <param name="message">
    /// A <see cref="ServiceControlMessage"/> object containing the arguments
    /// to start the service.
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
    void StartService(ServiceControlMessage message) {
      if (hosted_services_.Count > kMaxRunningServices) {
        logger_.Warn(
          "The limit of simultaneous running services has been reached");
        return;
      }

      var factory = new ServicesFactory(settings_);
      var service = factory.CreateService(message);
      var host = new RubyServiceHost(service, ruby_message_channel_);

      // Keep |hosted_services_| thread safe, since it is manipulated by more
      // than one thread (This thread and the thread that is running the
      // service).
      lock (hosted_service_mutex_) {
        hosted_services_.Add(service.Name, host);
      }

      // A try/catch block is used here to ensure the consistence of the
      // list of running services.
      try {
        var thread = new Thread(ThreadMain) {IsBackground = true};
        thread.Start(host);
      } catch (Exception exception) {
        lock (hosted_service_mutex_) {
          hosted_services_.Remove(service.Name);
        }
        logger_.Error(string.Format(StringResources.Log_MethodThrowsException,
          kClassName, "HostService"), exception);
      }
    }

    void StopService(ServiceControlMessage message) {
    }

    void ThreadMain(object o) {
      var host = o as RubyServiceHost;
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
      } catch (Exception exception) {
        logger_.Error(string.Format(StringResources.Log_MethodThrowsException,
          kClassName, "ThreadMain"), exception);
      }

      lock (hosted_service_mutex_) {
        hosted_services_.Remove(host.Service.Name);
      }
    }
  }
}
