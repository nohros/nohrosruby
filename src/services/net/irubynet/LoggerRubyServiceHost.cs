using System;
using System.Collections.Generic;
using Nohros.Data.Json;
using Nohros.Ruby.Protocol;

namespace Nohros.Ruby
{
  /// <summary>
  /// A implementtion of the <see cref="IRubyServiceHost"/> that send messages
  /// to a <see cref="IRubyLogger"/>.
  /// </summary>
  public class LoggerRubyServiceHost : IRubyServiceHost
  {
    readonly IRubyLogger logger_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="NopRubyServiceHost"/>.
    /// </summary>
    public LoggerRubyServiceHost(IRubyLogger logger) {
      logger_ = logger;
    }
    #endregion

    public IRubyLogger Logger {
      get { return logger_; }
    }

    public bool Send(IRubyMessage message) {
      return Send(message, new KeyValuePair<string, string>[0]);
    }

    public bool Send(IRubyMessage message,
      IEnumerable<KeyValuePair<string, string>> facts) {
      return Send(message.Id, message.Type, message.Message, message.Token,
        message.Sender, facts);
    }

    public bool Send(byte[] message_id, int type, byte[] message) {
      return Send(message_id, type, message,
        new KeyValuePair<string, string>[0]);
    }

    public bool Send(byte[] message_id, int type, byte[] message, string token) {
      return Send(message_id, type, message, token,
        new KeyValuePair<string, string>[0]);
    }

    /// <inheritdoc/>
    public bool Send(byte[] id, int type, byte[] message, byte[] destination) {
      return Send(id, type, message, destination,
        new KeyValuePair<string, string>[0]);
    }

    public bool Send(byte[] message_id, int type, byte[] message, string token,
      byte[] destination) {
      return Send(message_id, type, message, token, destination,
        new KeyValuePair<string, string>[0]);
    }

    public bool Send(byte[] message_id, int type, byte[] message,
      IEnumerable<KeyValuePair<string, string>> facts) {
      var msg = GetJsonStringBuilder(message_id, type, message)
        .ForEach(facts, (fact, builder) => builder
          .WriteMember(fact.Key, fact.Value))
        .ToString();
      logger_.Info(msg);
      return true;
    }

    public bool Send(byte[] message_id, int type, byte[] message, string token,
      IEnumerable<KeyValuePair<string, string>> facts) {
      var msg = GetJsonStringBuilder(message_id, type, message)
        .WriteMember("token", token)
        .ForEach(facts, (fact, builder) => builder
          .WriteMember(fact.Key, fact.Value))
        .ToString();
      logger_.Info(msg);
      return true;
    }

    /// <inheritdoc/>
    public bool Send(byte[] id, int type, byte[] message, byte[] destination,
      IEnumerable<KeyValuePair<string, string>> facts) {
      var msg = GetJsonStringBuilder(id, type, message)
        .WriteMember("destination", Convert.ToBase64String(destination))
        .ForEach(facts, (fact, builder) => builder
          .WriteMember(fact.Key, fact.Value))
        .ToString();
      logger_.Info(msg);
      return true;
    }

    public bool Send(byte[] message_id, int type, byte[] message, string token,
      byte[] destination, IEnumerable<KeyValuePair<string, string>> facts) {
      var msg = GetJsonStringBuilder(message_id, type, message)
        .WriteMember("token", token)
        .WriteMember("destination", Convert.ToBase64String(destination))
        .ForEach(facts, (fact, builder) => builder
          .WriteMember(fact.Key, fact.Value))
        .ToString();
      logger_.Info(msg);
      return true;
    }

    JsonStringBuilder GetJsonStringBuilder(byte[] id, int type, byte[] message) {
      return new JsonStringBuilder()
        .WriteMember("id", Convert.ToBase64String(id))
        .WriteMember("type", type)
        .WriteMember("message", Convert.ToBase64String(message));
    }
  }
}
