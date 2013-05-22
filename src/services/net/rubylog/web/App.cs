using System;
using Nohros.Concurrent;
using ServiceStack.WebHost.Endpoints;

namespace Nohros.Ruby.Logging
{
  public class App
  {
    readonly StatusManager manager_;
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
    public App(HttpPublisher publisher,
      MessageChannel message_channel,
      StatusManager manager,
      Settings settings) {
      publisher_ = publisher;
      message_channel_ = message_channel;
      settings_ = settings;
      manager_ = manager;

      message_channel.MessageReceived += OnMessageReceived;
    }
    #endregion

    void OnMessageReceived(LogMessage message) {
      publisher_.Broadcast(message);
      manager_.SetStatus(message.Application, GetStatus(message));
    }

    Status GetStatus(LogMessage message) {
      var status = new Status();
      switch (message.Level) {
        case "error":
          status.Type = StatusType.Error;
          break;

        case "info":
          status.Type = StatusType.Info;
          break;

        case "warn":
          status.Type = StatusType.Warn;
          break;

        default:
          status.Type = StatusType.Unknown;
          break;
      }
      return status;
    }

    public void Run() {
      message_channel_.Open(new BackgroundThreadFactory());
    }
  }
}
