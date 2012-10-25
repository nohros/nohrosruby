using System;
using System.Collections.Generic;
using System.Threading;
using Google.ProtocolBuffers;
using Nohros.Extensions;
using Nohros.Concurrent;
using Nohros.Ruby.Protocol;
using Nohros.Ruby.Protocol.Control;

namespace Nohros.Ruby
{
  /// <summary>
  /// .NET implementation of the <see cref="IRubyServiceHost"/> interface. This
  /// class is used to host a .NET based ruby services.
  /// </summary>
  internal class RubyServiceHost : IRubyServiceHost, IRubyMessageListener
  {
    const string kClassName = "Nohros.Ruby.RubyServiceHost";
    readonly RubyLogger logger_ = RubyLogger.ForCurrentProcess;

    readonly IRubyMessageChannel ruby_message_channel_;
    readonly IRubyService service_;
    readonly IRubyLogger service_logger_;
    readonly IRubySettings settings_;

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
    /// <param name="service_logger">
    /// A <see cref="IRubyLogger"/> object that can be used by the hosted
    /// service to log things.
    /// </param>
    public RubyServiceHost(IRubyService service, IRubyMessageChannel channel,
      IRubyLogger service_logger, IRubySettings settings) {
#if DEBUG
      if (service == null || channel == null) {
        throw new ArgumentNullException(service == null ? "service" : "sender");
      }
#endif
      settings_ = settings;
      service_ = service;
      ruby_message_channel_ = channel;
      service_logger_ = service_logger;
    }
    #endregion

    public void OnMessagePacketReceived(RubyMessagePacket packet) {
      if (logger_.IsDebugEnabled) {
        logger_.Debug("Received a message with token: "
          + packet.Message.HasToken);
      }
      // send the message to the service for processing.
      service_.OnMessage(packet.Message);
    }

    /// <inheritdoc/>
    public bool Send(byte[] message_id, int type, byte[] message, string token) {
      RubyMessage request = new RubyMessage.Builder()
        .SetId(ByteString.CopyFrom(message_id))
        .SetType(type)
        .SetMessage(ByteString.CopyFrom(message))
        .Build();
      return Send(request);
    }

    /// <inheritdoc/>
    public bool Send(IRubyMessage message) {
      if (logger_.IsDebugEnabled) {
        logger_.Debug("Sending a message with token " + message.Token);
      }
      return ruby_message_channel_.Send(message);
    }

    /// <inherithdoc/>
    public IRubyLogger Logger {
      get { return service_logger_; }
    }

    public bool Send(byte[] message_id, int type, byte[] message) {
      return Send(message_id, type, message, string.Empty);
    }

    public bool SendError(byte[] message_id, string error, int exception_code) {
      ExceptionMessage exception = new ExceptionMessage.Builder()
        .SetCode(exception_code)
        .SetMessage(error)
        .SetSource(service_.Facts.GetString(Strings.kServiceNameFact,
          Strings.kNodeServiceName))
        .Build();
      return SendError(message_id, new[] {exception});
    }

    public bool SendError(byte[] message_id, string error, int exception_code,
      Exception exception) {
      ExceptionMessage exception_message = new ExceptionMessage.Builder()
        .SetCode(exception_code)
        .SetMessage(error)
        .SetSource(service_.Facts.GetString(Strings.kServiceNameFact,
          Strings.kNodeServiceName))
        .AddData(KeyValuePairs.FromKeyValuePair("exception", exception.Message))
        .AddData(KeyValuePairs.FromKeyValuePair("backtrace",
          exception.StackTrace))
        .Build();
      return SendError(message_id, new[] {exception_message});
    }

    public bool SendError(byte[] message_id, int exception_code,
      Exception exception) {
      ExceptionMessage exception_message = new ExceptionMessage.Builder()
        .SetCode(exception_code)
        .SetMessage(exception.Message)
        .SetSource(service_.Facts.GetString(Strings.kServiceNameFact,
          Strings.kNodeServiceName))
        .AddData(KeyValuePairs.FromKeyValuePair("exception", exception.Message))
        .AddData(KeyValuePairs.FromKeyValuePair("backtrace",
          exception.StackTrace))
        .Build();
      return SendError(message_id, new[] {exception_message});
    }

    bool SendError(byte[] message_id, IEnumerable<ExceptionMessage> exceptions) {
      ErrorMessage error_message = new ErrorMessage.Builder()
        .AddRangeErrors(exceptions)
        .Build();
      RubyMessage message = new RubyMessage.Builder()
        .SetId(ByteString.CopyFrom(message_id))
        .SetType((int) NodeMessageType.kNodeError)
        .SetMessage(error_message.ToByteString())
        .Build();
      return Send(message);
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
      ruby_message_channel_.AddListener(this, Executors.ThreadPoolExecutor());
      Announce();

      Thread.CurrentThread.CurrentUICulture = settings_.Culture;
      service_.Start(this);

      logger_.Info("the following service has been finished: ", service_.Facts);
    }

    void Announce() {
      // Tell the service node that we are hosting a new service.
      AnnounceMessage.Builder builder = new AnnounceMessage.Builder();
      foreach (KeyValuePair<string, string> fact in service_.Facts) {
        builder.AddFacts(
          new KeyValuePair.Builder()
            .SetKey(fact.Key)
            .SetValue(fact.Value));
      }

      RubyMessage message = new RubyMessage.Builder()
        .SetToken(StringResources.kAnnounceMessageToken)
        .SetType((int) NodeMessageType.kNodeAnnounce)
        .SetMessage(builder.Build().ToByteString())
        .Build();
      Send(message);

      if (logger_.IsDebugEnabled) {
        logger_.Debug("announcing service", service_.Facts);
      }
    }

    /// <inherithdoc/>
    public IRubyService Service {
      get { return service_; }
    }
  }
}
