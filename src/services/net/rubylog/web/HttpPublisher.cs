using System;
using Microsoft.AspNet.SignalR;

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
  }
}