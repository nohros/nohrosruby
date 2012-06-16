using System;
using Nohros.Data.Json;

namespace Nohros.Ruby.Service.Net
{
  /// <summary>
  /// A implementation of the <see cref="IRubyMessageSender"/> class that
  /// forward sent message to the configured application logger.
  /// <remarks>
  /// The messages is forwarded to the logger using the INFO log level.
  /// </remarks>
  /// </summary>
  internal class LoggerMessageSender : IRubyMessageSender
  {
    IRubyLogger logger;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="LoggerMessageSender"/>
    /// class.
    /// </summary>
    public LoggerMessageSender() {
      logger = RubyLogger.ForCurrentProcess;
    }
    #endregion

    /// <inheritdoc/>
    public bool Send(IRubyMessage message) {
      JsonStringBuilder json_string_builder = new JsonStringBuilder();
      json_string_builder
        .WriteBeginObject()
        .WriteMember("id", message.Id)
        .WriteMember("token", message.Token)
        .WriteMember("type", message.Type)
        .WriteEndObject();
      logger.Info(json_string_builder.ToString());
      return true;
    }
  }
}
