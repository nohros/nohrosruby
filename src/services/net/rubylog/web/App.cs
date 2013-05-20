using System;
using Nohros.Concurrent;

namespace Nohros.Ruby.Logging
{
  public class App
  {
    readonly MessageChannel message_channel_;
    readonly HttpPublisher publisher_;
    readonly Settings settings_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="App"/> class using the
    /// specified http publisher, message channel and application settings.
    /// </summary>
    /// <param name="publisher">
    /// A <see cref="HttpPublisher"/> object that can be used to publish
    /// messages HTTP peers.
    /// </param>
    /// <param name="message_channel">
    /// A <see cref="MessageChannel"/> object that can be used to retrieve
    /// message from the logging server.
    /// </param>
    /// <param name="settings">
    /// A <see cref="Settings"/> object containing the user defined settings.
    /// </param>
    public App(HttpPublisher publisher, MessageChannel message_channel,
      Settings settings) {
      publisher_ = publisher;
      message_channel_ = message_channel;
      settings_ = settings;

      message_channel.MessageReceived += publisher.Broadcast;
    }
    #endregion

    public void Run() {
      message_channel_.Open(new BackgroundThreadFactory());
    }
  }
}
