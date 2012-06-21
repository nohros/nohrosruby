using System;
using Nohros.Concurrent;
using Nohros.Resources;
using Nohros.Ruby.Protocol;

namespace Nohros.Ruby
{
  /// <summary>
  /// .NET implementation of the <see cref="IRubyServiceHost"/> interface. This
  /// class is used to host a .NET based ruby services.
  /// </summary>
  internal class RubyServiceHost : IRubyServiceHost, IRubyMessageListener
  {
    const string kClassName = "Nohros.Ruby.RubyServiceHost";
    readonly IRubyLogger logger = RubyLogger.ForCurrentProcess;

    readonly IRubyMessageChannel ruby_message_channel_;
    readonly IRubyService service_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="RubyServiceHost"/> class
    /// by using the specified service to hostm message sender and listener.
    /// </summary>
    /// <param name="service">
    /// The service to host.
    /// </param>
    /// <param name="channel">
    /// A <see cref="IRubyMessageSender"/> that can be used to send messages to
    /// the ruby service node.
    /// </param>
    public RubyServiceHost(IRubyService service, IRubyMessageChannel channel) {
#if DEBUG
      if (service == null || channel == null) {
        throw new ArgumentNullException(service == null ? "service" : "sender");
      }
#endif
      service_ = service;
      ruby_message_channel_ = channel;
    }
    #endregion

    public void OnMessagePacketReceived(RubyMessagePacket packet) {
      if (string.Compare(packet.Header.Service, service_.Name,
        StringComparison.OrdinalIgnoreCase) == 0) {
        // TODO(sender): track the sender and reply back
        service_.OnMessage(packet.Message);
      }
    }

    /// <inheritdoc/>
    public bool Send(IRubyMessage message) {
      return ruby_message_channel_.Send(message);
    }

    /// <summary>
    /// Starts the hosted service.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The hosted service runs into a dedicated thread. The thread where
    /// this code is running is used to send/receive messages to/from the
    /// service.
    /// </para>
    /// <para>
    /// This method does not return until the running hosted service have
    /// finished your execution.
    /// </para>
    /// <para>
    /// If the service throws any exception this is propaggated to the
    /// caller and the service is forced to stop.
    /// </para>
    /// </remarks>
    public void Start() {
      ruby_message_channel_.AddListener(this, Executors.SameThreadExecutor());
      service_.Start(this);
    }

    /// <inherithdoc/>
    public IRubyService Service {
      get { return service_; }
    }
  }
}
