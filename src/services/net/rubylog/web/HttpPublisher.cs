using System;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR;
using Nohros.Data.Json;

namespace Nohros.Ruby.Logging
{
  /// <summary>
  /// Publish log message over the HTTP channel.
  /// </summary>
  /// <remarks>
  /// The messages are serialized using the JSON format.
  /// </remarks>
  public class HttpPublisher : PersistentConnection
  {
    readonly IPersistentConnectionContext context_;
    readonly IDictionary<string, string> subscription_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="HttpPublisher"/> class.
    /// </summary>
    public HttpPublisher() {
      subscription_ =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

      // To use the connection outside the persistent connection we need to
      // use the persistent connection context.
      context_ =
        GlobalHost.ConnectionManager.GetConnectionContext<HttpPublisher>();
    }
    #endregion

    /// <summary>
    /// Broadcast <paramref name="message"/> to all peers that has subscribed
    /// to the log's application feed.
    /// </summary>
    /// <param name="message">
    /// The message to broadcast.
    /// </param>
    public void Broadcast(LogMessage message) {
      context_.Connection.Broadcast(
        new JsonStringBuilder()
          .WriteBeginObject()
          .WriteMember("app", message.Application)
          .WriteMember("level", message.Level)
          .WriteMember("reason", message.Reason)
          .WriteMember("user", message.User)
          .WriteMember("timestamp", message.TimeStamp)
          .ForEach(message.CategorizationList,
            (category, builder) =>
              builder.WriteMember(category.Key, category.Value))
          .ToString());
    }
  }
}
