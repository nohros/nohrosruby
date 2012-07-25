using System;
using System.Collections.Generic;
using System.Threading;
using Google.ProtocolBuffers;
using Nohros.Concurrent;
using Nohros.Data.Json;
using Nohros.Resources;
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

    readonly object hosted_service_mutex_;
    readonly IRubyLogger logger_;
    readonly Dictionary<string, ResponseMessageHandler> messages_tokens_;
    readonly IRubyMessageChannel ruby_message_channel_;
    readonly IRubySettings settings_;
    int running_services_count_;

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
      hosted_service_mutex_ = new object();
      logger_ = RubyLogger.ForCurrentProcess;
      settings_ = settings;
      running_services_count_ = 0;
      messages_tokens_ = new Dictionary<string, ResponseMessageHandler>();

      InitMessageTokens();
    }
    #endregion

    /// <inheritdoc/>
    public virtual void Run() {
      Run(string.Empty);
    }

    /// <inheritdoc/>
    public virtual void Run(string command_line_string) {
      ruby_message_channel_.Open();
      ruby_message_channel_.AddListener(this, Executors.SameThreadExecutor());

      // Query the service node for the log aggregator service.
      QueryLogAggregatorService();

      // TODO(neylor.silva) Remove the code above as soon as the c++
      // implementation is done.
      //
      
      // Simulate the receiving of a response for the log aggregator query.
      SimulateQueryResponse();
    }

    /// <inheritdoc/>
    public void OnMessagePacketReceived(RubyMessagePacket packet) {
      switch (packet.Message.Type) {
        case (int) NodeMessageType.kServiceControl:
          OnServiceControlMessage(packet.Message.Message);
          break;

        case (int) NodeMessageType.kNodeResponse:
          OnResponseMessage(packet.Message.Message);
          break;
      }
    }

    /// <inheritdoc/>
    public virtual IRubyMessageChannel RubyMessageChannel {
      get { return ruby_message_channel_; }
    }

    void InitMessageTokens() {
      messages_tokens_.Add(kLogAggregatorQuery, OnLogAggregatorQueryReseponse);
    }

    /// <inheritdoc/>
    void OnServiceControlMessage(ByteString message) {
      try {
        ServiceControlMessage service_control_message =
          ServiceControlMessage.ParseFrom(message);
        switch ((ServiceControlMessageType) service_control_message.Type) {
          case ServiceControlMessageType.kServiceControlStart:
            StartService(service_control_message);
            break;

          case ServiceControlMessageType.kServiceControlStop:
            StopService(service_control_message);
            break;
        }
      } catch (Exception exception) {
        logger_.Error(string.Format(StringResources.Log_MethodThrowsException,
          kClassName, "OnMessagePacketReceived"), exception);
      }
    }

    void OnResponseMessage(ByteString message) {
      try {
        ResponseMessage response = ResponseMessage.ParseFrom(message);
        RubyMessage request = response.Request;

        // Every request issued by this class should contains a token that
        // maps to a message handler (defined at InitMessageHandler method).
        ResponseMessageHandler handler;
        if (messages_tokens_.TryGetValue(request.Token, out handler)) {
          handler(response);
        } else {
          // A handler was not found. Either, the request was not issued by
          // this class or the request was modified. There is nothing we can do
          // with that message.
          logger_.Warn("Could not found a reseponse handler for the received"
            + "message."
              + new JsonStringBuilder()
                .WriteBeginObject()
                .WriteMemberName("message")
                .WriteMember("id", request.Id)
                .WriteMember("token", request.Token)
                .WriteMember("type", request.Type));
        }
      } catch (Exception exception) {
        logger_.Error(
          string.Format(StringResources.Log_MethodThrowsException, kClassName,
            "OnResponseMessage"), exception);
      }
    }

    // Query reseponses message handlers ------------------------------------
    //

    void QueryLogAggregatorService() {
      QueryMessage query = new QueryMessage.Builder()
        .SetType(QueryMessageType.kQueryFind)
        .AddFacts(KeyValuePairs.FromKeyValuePair(Strings.kMessageUUIDFact,
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
        .AddFacts(KeyValuePairs.FromKeyValuePair(Strings.kServiceNameFact,
          Strings.kNodeServiceName))
        .SetSize(message.SerializedSize)
        .Build();

      RubyMessagePacket packet = new RubyMessagePacket.Builder()
        .SetHeader(header)
        .SetHeaderSize(header.SerializedSize)
        .SetMessage(message)
        .SetSize(header.SerializedSize + message.SerializedSize + 4)
        .Build();

      ruby_message_channel_.Send(packet);
    }

    void SimulateQueryResponse() {
      QueryMessage query = new QueryMessage.Builder()
        .SetType(QueryMessageType.kQueryFind)
        .AddFacts(KeyValuePairs.FromKeyValuePair(Strings.kMessageUUIDFact,
          "cfa950a0ca0611e19b230800200c9a66"))
        .Build();

      RubyMessage message = new RubyMessage.Builder()
        .SetId(0)
        .SetToken(kLogAggregatorQuery)
        .SetType((int)NodeMessageType.kNodeQuery)
        .SetMessage(query.ToByteString())
        .Build();

      RubyMessageHeader header = new RubyMessageHeader.Builder()
        .SetId(message.Id)
        .AddFacts(KeyValuePairs.FromKeyValuePair(Strings.kServiceNameFact,
          Strings.kNodeServiceName))
        .SetSize(message.SerializedSize)
        .Build();

      RubyMessagePacket packet = new RubyMessagePacket.Builder()
        .SetHeader(header)
        .SetHeaderSize(header.SerializedSize)
        .SetMessage(message)
        .SetSize(header.SerializedSize + message.SerializedSize + 4)
        .Build();

      ResponseMessage response = new ResponseMessage.Builder()
        .AddReponses(KeyValuePairs.FromKeyValuePair(
          Strings.kHostServiceFact, "192.168.203.252:8520"))
        .SetRequest(message)
        .Build();

      RubyMessage response_message = new RubyMessage.Builder()
        .SetId(0)
        .SetToken(Strings.kNodeResponseToken)
        .SetType((int)NodeMessageType.kNodeResponse)
        .SetMessage(response.ToByteString())
        .Build();

      RubyMessageHeader response_header = new RubyMessageHeader.Builder()
        .SetId(response_message.Id)
        .AddFacts(KeyValuePairs.FromKeyValuePair(Strings.kServiceNameFact,
          Strings.kNodeServiceName))
        .SetSize(response_message.SerializedSize)
        .Build();

      RubyMessagePacket response_packet = new RubyMessagePacket.Builder()
        .SetHeader(response_header)
        .SetHeaderSize(response_header.SerializedSize)
        .SetMessage(response_message)
        .SetSize(response_header.SerializedSize + response_message.SerializedSize + 4)
        .Build();

      OnMessagePacketReceived(response_packet);
    }


    void OnLogAggregatorQueryReseponse(ResponseMessage response) {
      IList<KeyValuePair> responses = response.ReponsesList;
      int index = Find(Strings.kHostServiceFact, responses);
      if (index != -1) {
        string log_aggregator_address = responses[index].Value;
        ZMQ.Context context = new Context();
        AggregatorService aggregator = new AggregatorService(context,
          log_aggregator_address);
        if (aggregator.Configure()) {
          settings_.AggregatorService = aggregator;
        }
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
        logger_.Error(string.Format(StringResources.Log_MethodThrowsException,
          kClassName, "HostService"), exception);
      }
    }

    void StopService(ServiceControlMessage message) {
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
      var factory = new ServicesFactory(settings_);
      var service = factory.CreateService(message);
      var host = new RubyServiceHost(service, ruby_message_channel_, settings_);

      // A try/catch block is used here to ensure the consistence of the
      // list of running services and to isolate one service from another.
      try {
        ++running_services_count_;
        host.Start();
      } catch (Exception exception) {
        logger_.Error(string.Format(StringResources.Log_MethodThrowsException,
          kClassName, "ServiceThreadMain"), exception);
      }
      --running_services_count_;
    }

    /// <summary>
    /// Converst a list of <see cref="KeyValuePair"/> to a dictionary of
    /// strings.
    /// </summary>
    /// <param name="key_value_pairs">
    /// The list of <see cref="KeyValuePair"/> to be converted.
    /// </param>
    /// <returns></returns>
    IDictionary<string, string> ListToDictionary(
      IList<KeyValuePair> key_value_pairs) {
      Dictionary<string, string> dictionary =
        new Dictionary<string, string>(key_value_pairs.Count);
      for (int i = 0, j = key_value_pairs.Count; i < j; i++) {
        KeyValuePair pair = key_value_pairs[i];
        dictionary[pair.Key] = pair.Value;
      }
      return dictionary;
    }

    int Find(string pattern, IList<KeyValuePair> key_value_pairs) {
      for (int i = 0, j = key_value_pairs.Count; i < j; i++) {
        KeyValuePair key_value_pair = key_value_pairs[i];
        if (string.Compare(key_value_pair.Key, pattern, StringComparison.OrdinalIgnoreCase) == 0) {
          return i;
        }
      }
      return -1;
    }
  }
}
