using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Google.ProtocolBuffers;
using Nohros.Concurrent;
using Nohros.Extensions;
using Nohros.Logging;
using Nohros.Ruby.Extensions;
using R = Nohros.Resources.StringResources;
using Nohros.Ruby.Protocol;
using Nohros.Ruby.Protocol.Control;
using ZMQ;
using Exception = System.Exception;

namespace Nohros.Ruby
{
  /// <summary>
  /// Provides a skeletal implementation of the <see cref="IRubyProcess"/>
  /// interface to reduce the effort required to implement it.
  /// </summary>
  internal abstract class AbstractRubyProcess : IRubyProcess
  {
    delegate void ResponseMessageHandler(ResponseMessage response);

    const string kClassName = "Nohros.Ruby.AbstractRubyProcess";
    const string kLogAggregatorQuery = "query_logger_aggregator_service";

    const int kMaxRunningServices = 10;
    readonly ForwardingAggregatorService forwarding_aggregator_service_;

    readonly List<IRubyServiceHost> hosts_;
    readonly IRubyLogger logger_;
    readonly Dictionary<string, ResponseMessageHandler> messages_tokens_;
    readonly IRubyMessageChannel ruby_message_channel_;
    readonly IRubySettings settings_;
    volatile int running_services_count_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="AbstractRubyProcess"/>
    /// class by using the specified <see cref="ProcessMessageChannel"/> object.
    /// </summary>
    protected AbstractRubyProcess(IRubySettings settings,
      IRubyMessageChannel ruby_message_channel) {
      ruby_message_channel_ = ruby_message_channel;
      logger_ = RubyLogger.ForCurrentProcess;
      settings_ = settings;
      running_services_count_ = 0;
      hosts_ = new List<IRubyServiceHost>();
      messages_tokens_ = new Dictionary<string, ResponseMessageHandler>();
      forwarding_aggregator_service_ =
        new ForwardingAggregatorService(new LoggerAggregatorService());
      InitMessageTokens();
    }
    #endregion

    /// <inheritdoc/>
    public virtual void Run() {
      Run(string.Empty);
    }

    /// <inheritdoc/>
    public virtual void Run(string command_line_string) {
      ruby_message_channel_.AddListener(this, Executors.SameThreadExecutor());

      // TODO(neylor.silva) Remove the code above as soon as the c++
      // implementation is done.
      //
      // Simulate the receiving of a response for the log aggregator query.
      // SimulateQueryResponse();
    }

    /// <inheritdoc/>
    public virtual void OnMessagePacketReceived(RubyMessagePacket packet) {
      switch (packet.Message.Type) {
        case (int) NodeMessageType.kServiceControl:
          OnServiceControlMessage(packet.Message.Message);
          break;

        case (int) NodeMessageType.kNodeResponse:
          OnResponseMessage(packet.Message);
          break;
      }
    }

    public virtual void Exit() {
      foreach (var host in hosts_) {
        host.Shutdown();
      }
      ruby_message_channel_.Close();
    }

    /// <inheritdoc/>
    //public virtual IRubyMessageChannel ProcessMessageChannel {
    //get { return ruby_message_channel_; }
    //}
    void InitMessageTokens() {
      messages_tokens_.Add(kLogAggregatorQuery, OnLogAggregatorQueryReseponse);
    }

    /// <inheritdoc/>
    void OnServiceControlMessage(ByteString message) {
      try {
        ServiceControlMessage service_control_message =
          ServiceControlMessage.ParseFrom(message);
        switch (service_control_message.Type) {
          case ServiceControlMessageType.kServiceControlStart:
            StartService(service_control_message);
            break;

          case ServiceControlMessageType.kServiceControlStop:
            StopService(service_control_message);
            break;
        }
      } catch (Exception exception) {
        logger_.Error(string.Format(R.Log_MethodThrowsException,
          kClassName, "OnMailboxMessagePacketReceived"), exception);
      }
    }

    protected void OnResponseMessage(RubyMessage message) {
      try {
        ResponseMessage response = ResponseMessage.ParseFrom(message.Message);
        ResponseMessageHandler handler;

        // The ID of each request issued by this class should maps to a message
        // handler (defined at InitMessageHandler method).
        string request_message_token = message.Id.ToByteArray().AsString();
        if (messages_tokens_.TryGetValue(request_message_token, out handler)) {
          handler(response);
        } else {
          // A handler was not found. Either, the request was not issued by
          // this class or the request was modified. There is nothing we can do
          // with that message.
          logger_.Warn(string.Format(
            Resources.RubyProcess_ResponseMessageHandle_NotFound,
            request_message_token));
        }
      } catch (Exception exception) {
        logger_.Error(
          string.Format(R.Log_MethodThrowsException, kClassName,
            "OnResponseMessage"), exception);
      }
    }

    // Query reseponses message handlers ------------------------------------
    //

    protected void QueryLogAggregatorService() {
      QueryMessage query = new QueryMessage.Builder()
        .SetType(QueryMessageType.kQueryFind)
        .AddFacts(
          KeyValuePairs.FromKeyValuePair(RubyStrings.kMessageIDFact,
            "cfa950a0ca0611e19b230800200c9a66"))
        .Build();

      RubyMessage message = new RubyMessage.Builder()
        .SetToken(kLogAggregatorQuery)
        .SetType((int) NodeMessageType.kNodeQuery)
        .SetMessage(query.ToByteString())
        .SetId(ByteString.CopyFrom(kLogAggregatorQuery.AsBytes(Encoding.ASCII)))
        .Build();
      ruby_message_channel_.Send(message, new[] {
        new KeyValuePair<string, string>(RubyStrings.kServiceNameFact,
          Strings.kNodeServiceName)
      });
    }

    /*void SimulateQueryResponse() {
      QueryMessage query = new QueryMessage.Builder()
        .SetType(QueryMessageType.kQueryFind)
        .AddFacts(
          KeyValuePairs.FromKeyValuePair(RubyStrings.kServiceUIDFact,
            "cfa950a0ca0611e19b230800200c9a66"))
        .Build();

      RubyMessage message = new RubyMessage.Builder()
        .SetId(0)
        .SetToken(kLogAggregatorQuery)
        .SetType((int) NodeMessageType.kNodeQuery)
        .SetMessage(query.ToByteString())
        .Build();

      RubyMessageHeader header = new RubyMessageHeader.Builder()
        .SetId(message.Id)
        .AddFacts(
          KeyValuePairs.FromKeyValuePair(RubyStrings.kServiceNameFact,
            Strings.kNodeServiceName))
        .SetSize(message.SerializedSize)
        .Build();

      ResponseMessage response = new ResponseMessage.Builder()
        .AddReponses(KeyValuePairs.FromKeyValuePair(
          Strings.kHostServiceFact, "192.168.203.252:8520"))
        .SetRequest(message)
        .Build();

      RubyMessage response_message = new RubyMessage.Builder()
        .SetId(0)
        .SetToken(RubyStrings.kNodeResponseToken)
        .SetType((int) NodeMessageType.kNodeResponse)
        .SetMessage(response.ToByteString())
        .Build();

      RubyMessageHeader response_header = new RubyMessageHeader.Builder()
        .SetId(response_message.Id)
        .AddFacts(
          KeyValuePairs.FromKeyValuePair(RubyStrings.kServiceNameFact,
            Strings.kNodeServiceName))
        .SetSize(response_message.SerializedSize)
        .Build();

      RubyMessagePacket response_packet = new RubyMessagePacket.Builder()
        .SetHeader(response_header)
        .SetHeaderSize(response_header.SerializedSize)
        .SetMessage(response_message)
        .SetSize(response_header.SerializedSize +
          response_message.SerializedSize + 4)
        .Build();

      OnMailboxMessagePacketReceived(response_packet);
    }*/

    void OnLogAggregatorQueryReseponse(ResponseMessage response) {
      IList<KeyValuePair> responses = response.ReponsesList;
      int index = Find(Strings.kHostServiceFact, responses);
      if (index != -1) {
        string log_aggregator_address = responses[index].Value;
        var context = new Context();
        ILogger logger = RubyLogger.ForCurrentProcess.Logger;

        // Ensure that the current configured logger is not an instance of the
        // AggregatorLogger class.
        if (logger is AggregatorLogger) {
          logger.Warn(Resources.log_aggregator_circular_reference);
          return;
        }

        var aggregator =
          new AggregatorService(context, log_aggregator_address, logger);
        if (aggregator.Configure()) {
          forwarding_aggregator_service_.BackingAggregatorService = aggregator;
        }

        RubyLogger.ForCurrentProcess.BackingLogger =
          new AggregatorLogger(Strings.kServiceHostServiceName, settings_,
            aggregator);
      }
    }


    // Service control messages ---------------------------------------------
    //

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
      if (running_services_count_ > kMaxRunningServices) {
        logger_.Warn(
          "The limit of simultaneous running services has been reached");
        return;
      }

      // A try/catch block is used here to ensure the consistence of the
      // list of running services.
      try {
        var thread = new Thread(ServiceThreadMain) {IsBackground = true};
        thread.Start(message);
      } catch (Exception exception) {
        logger_.Error(string.Format(R.Log_MethodThrowsException,
          kClassName, "HostService"), exception);
      }
    }

    void StopService(ServiceControlMessage message) {
      // TODO: implement the service stop.
    }

    /// <summary>
    /// Creates and starts a service in a dedicated thread.
    /// </summary>
    /// <param name="o">
    /// A <see cref="ServiceControlMessage"/> containing information about the
    /// service to be started.
    /// </param>
    void ServiceThreadMain(object o) {
      var message = o as ServiceControlMessage;
#if DEBUG
      if (message == null) {
        throw new ArgumentException(
          "object 'o' is not an instance of the ServiceControlMessage class");
      }
#endif
      ++running_services_count_;
      // A try/catch block is used here to ensure the consistence of the
      // list of running services and to handle exceptions taht may occur
      // during service initialization and is not properly handled by
      // the service.
      try {
        var factory = new ServicesFactory(settings_);
        IRubyService service = factory.CreateService(message);
        string service_name = service.Facts
          .GetString(Strings.kServiceNameFact, Strings.kNodeServiceName);
        var aggregator_logger = new AggregatorLogger(service_name, settings_,
          forwarding_aggregator_service_);
        var host = new RubyServiceHost(service, ruby_message_channel_,
          aggregator_logger, settings_);
        ruby_message_channel_.AddListener(host, Executors.SameThreadExecutor());

        // keep track the running services.
        lock (hosts_) {
          hosts_.Add(host);
        }

        // start the host and its associated service.
        host.ServiceHostStart += OnServiceHostStart;
        host.Start();
      } catch (Exception exception) {
        logger_.Error(string.Format(R.Log_MethodThrowsException,
          kClassName, "ServiceThreadMain"), exception);
      }
      --running_services_count_;
    }

    int Find(string pattern, IList<KeyValuePair> key_value_pairs) {
      for (int i = 0, j = key_value_pairs.Count; i < j; i++) {
        KeyValuePair key_value_pair = key_value_pairs[i];
        if (
          string.Compare(key_value_pair.Key, pattern,
            StringComparison.OrdinalIgnoreCase) == 0) {
          return i;
        }
      }
      return -1;
    }

    void OnServiceHostStart(IRubyServiceHost host) {
      Listeners.SafeInvoke<ServiceHostStartEventHandler>(ServiceHostStart,
        handler => handler(host));
    }

    public event ServiceHostStartEventHandler ServiceHostStart;
  }
}
