using System;
using System.Collections.Generic;
using System.Threading;
using Google.ProtocolBuffers;
using Nohros.Extensions;
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

    readonly IRubyMessageSender ruby_message_sender_;
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
    /// <param name="sender">
    /// A <see cref="IRubyMessageSender"/> that can be used to send messages to
    /// the ruby service node.
    /// </param>
    /// <param name="service_logger">
    /// A <see cref="IRubyLogger"/> object that can be used by the hosted
    /// service to log things.
    /// </param>
    public RubyServiceHost(IRubyService service, IRubyMessageSender sender,
      IRubyLogger service_logger, IRubySettings settings) {
#if DEBUG
      if (service == null || sender == null || settings == null ||
        service_logger_ == null) {
        throw new ArgumentNullException(service == null ? "service" : "sender");
      }
#endif
      settings_ = settings;
      service_ = service;
      ruby_message_sender_ = sender;
      service_logger_ = service_logger;
    }
    #endregion

    public void OnMessagePacketReceived(RubyMessagePacket packet) {
      if (logger_.IsDebugEnabled) {
        logger_.Debug("Received a message with token: "
          + packet.Message.HasToken);
      }
      // TODO(neylor.silva): This method is called for any message that is
      // received by channel. We need to filter the control messages and
      // dispatch to the service only the messages directed to them.
      service_.OnMessage(packet.Message);
    }

    /// <inheritdoc/>
    public bool Send(IRubyMessage message,
      IEnumerable<KeyValuePair<string, string>> facts) {
      return ruby_message_sender_.Send(message, facts);
    }

    /// <inheritdoc/>
    public bool Send(byte[] message_id, int type, byte[] message,
      byte[] destination, IEnumerable<KeyValuePair<string, string>> facts) {
      return ruby_message_sender_.Send(message_id, type, message, destination,
        facts);
    }

    /// <inheritdoc/>
    public bool Send(byte[] message_id, int type, byte[] message, string token) {
      return ruby_message_sender_.Send(message_id, type, message, token);
    }

    /// <inheritdoc/>
    public bool Send(byte[] message_id, int type, byte[] message, string token,
      byte[] destination) {
      return ruby_message_sender_.Send(message_id, type, message, token,
        destination);
    }

    /// <inheritdoc/>
    public bool Send(byte[] message_id, int type, byte[] message, string token,
      byte[] destination, IEnumerable<KeyValuePair<string, string>> facts) {
      return ruby_message_sender_.Send(message_id, type, message, token,
        destination, facts);
    }

    /// <inheritdoc/>
    public bool Send(byte[] message_id, int type, byte[] message, string token,
      IEnumerable<KeyValuePair<string, string>> facts) {
      return ruby_message_sender_.Send(message_id, type, message, token, facts);
    }

    /// <inheritdoc/>
    public bool Send(byte[] message_id, int type, byte[] message) {
      return ruby_message_sender_.Send(message_id, type, message);
    }

    /// <inheritdoc/>
    public bool Send(byte[] message_id, int type, byte[] message,
      IEnumerable<KeyValuePair<string, string>> facts) {
      return ruby_message_sender_.Send(message_id, type, message, facts);
    }

    public bool Send(byte[] message_id, int type, byte[] message,
      byte[] destination) {
      return ruby_message_sender_.Send(message_id, type, message, destination);
    }

    /// <inheritdoc/>
    public bool Send(IRubyMessage message) {
      if (logger_.IsDebugEnabled) {
        logger_.Debug("Sending a message with token " + message.Token);
      }
      return ruby_message_sender_.Send(message);
    }

    /// <inherithdoc/>
    public IRubyLogger Logger {
      get { return service_logger_; }
    }

    public byte[] FormatErrorMessage(byte[] message_id, int exception_code,
      string error,
      byte[] destination) {
      ExceptionMessage exception = new ExceptionMessage.Builder()
        .SetCode(exception_code)
        .SetMessage(error)
        .SetSource(service_.Facts.GetString(Strings.kServiceNameFact,
          Strings.kNodeServiceName))
        .Build();
      return FormatErrorMessage(message_id, destination, new[] {exception});
    }

    public byte[] FormatErrorMessage(byte[] message_id, int exception_code,
      string error,
      byte[] destination, Exception exception) {
      ExceptionMessage exception_message = new ExceptionMessage.Builder()
        .SetCode(exception_code)
        .SetMessage(error)
        .SetSource(service_.Facts.GetString(Strings.kServiceNameFact,
          Strings.kNodeServiceName))
        .AddData(KeyValuePairs.FromKeyValuePair("exception", exception.Message))
        .AddData(KeyValuePairs.FromKeyValuePair("backtrace",
          exception.StackTrace))
        .Build();
      return FormatErrorMessage(message_id, destination,
        new[] {exception_message});
    }

    /// <inheritdoc/>
    public bool Send(byte[] message_id, int type, byte[] message,
      byte[] destination, string token) {
      RubyMessage request = new RubyMessage.Builder()
        .SetId(ByteString.CopyFrom(message_id))
        .SetType(type)
        .SetSender(ByteString.CopyFrom(destination))
        .SetMessage(ByteString.CopyFrom(message))
        .Build();
      return Send(request);
    }

    public byte[] FormatErrorMessage(byte[] message_id, int exception_code,
      byte[] destination, Exception exception) {
      return FormatErrorMessage(message_id, destination,
        new[] {GetErrorMessage(exception_code, exception)});
    }

    public byte[] FormatErrorMessage(byte[] message_id, int exception_code,
      Exception exception) {
      return FormatErrorMessage(message_id,
        new[] {GetErrorMessage(exception_code, exception)});
    }

    ExceptionMessage GetErrorMessage(int exception_code, Exception exception) {
      return new ExceptionMessage.Builder()
        .SetCode(exception_code)
        .SetMessage(exception.Message)
        .SetSource(service_.Facts.GetString(Strings.kServiceNameFact,
          Strings.kNodeServiceName))
        .AddData(KeyValuePairs.FromKeyValuePair("exception", exception.Message))
        .AddData(KeyValuePairs.FromKeyValuePair("backtrace",
          exception.StackTrace))
        .Build();
    }

    byte[] FormatErrorMessage(byte[] message_id, byte[] destination,
      IEnumerable<ExceptionMessage> exceptions) {
      return GetErrorMessageBuilder(message_id, exceptions)
        .SetSender(ByteString.CopyFrom(destination))
        .Build()
        .ToByteArray();
    }

    byte[] FormatErrorMessage(byte[] message_id,
      IEnumerable<ExceptionMessage> exceptions) {
      return GetErrorMessageBuilder(message_id, exceptions)
        .Build()
        .ToByteArray();
    }

    RubyMessage.Builder GetErrorMessageBuilder(byte[] message_id,
      IEnumerable<ExceptionMessage> exceptions) {
      ErrorMessage error_message = new ErrorMessage.Builder()
        .AddRangeErrors(exceptions)
        .Build();
      return new RubyMessage.Builder()
        .SetId(ByteString.CopyFrom(message_id))
        .SetType((int) NodeMessageType.kNodeError)
        .SetMessage(error_message.ToByteString());
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
      Announce();

      Thread.CurrentThread.CurrentUICulture = settings_.Culture;
      service_.Start(this);

      logger_.Info("the following service has been finished: ", service_.Facts);
    }

    void Announce() {
      // Tell the service node that we are hosting a new service.
      AnnounceMessage.Builder builder = new AnnounceMessage.Builder();
      foreach (KeyValuePair<string, string> fact in service_.Facts) {
        builder
          .AddFacts(KeyValuePairs.FromKeyValuePair(fact));
        //.AddFacts(KeyValuePairs.FromKeyValuePair(Strings.kHostServiceFact,
        //ruby_message_sender_.Endpoint));
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
