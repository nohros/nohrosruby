using System;
using Google.ProtocolBuffers;
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
    readonly IRubyLogger logger_ = RubyLogger.ForCurrentProcess;

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
      // send the message to the service for processing.
      IRubyMessage message = service_.OnMessage(packet.Message);
      RubyMessage response = message as RubyMessage ??
        RubyMessage.ParseFrom(message.ToByteArray());

      // Create the repy packed using the service processing result.
      int message_size = message.ToByteArray().Length;
      RubyMessageHeader header = new RubyMessageHeader.Builder()
        .SetSize(message_size)
        .Build();

      int header_size = header.SerializedSize;
      RubyMessagePacket reply = new RubyMessagePacket.Builder()
        .SetHeader(header)
        .SetHeaderSize(header.SerializedSize)
        .SetMessage(response)
        .SetSize(header_size + 2 + message_size)
        .Build();

      if (!ruby_message_channel_.Send(reply)) {
        logger_.Warn("Reply message cannot be sent: " +
          response.ToByteString().ToBase64());
      }
    }

    /// <inheritdoc/>
    public bool Send(IRubyMessage message, string service) {
      return ruby_message_channel_.Send(message, service);
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
      ruby_message_channel_.AddListener(this, Executors.ThreadPoolExecutor(),
        service_.Name);
      service_.Start(this);
    }

    /// <inherithdoc/>
    public IRubyService Service {
      get { return service_; }
    }
  }
}
