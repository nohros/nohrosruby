﻿using System;
using System.Collections.Generic;
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

    /// <inheritdoc/>
    public bool Send(IRubyMessage message) {
      logger_.Info("Send => id:" + message.Id + ",type:" + message.Type
        + ",token:" + message.Token);
      return true;
    }

    /// <inheritdoc/>
    public bool Send(byte[] id, int type, byte[] message, byte[] destination) {
      logger_.Info("Send => id:" + id + ",type:" + type);
      return true;
    }

    public IRubyLogger Logger {
      get { return logger_; }
    }

    /// <inheritdoc/>
    public bool SendError(byte[] message_id, int exception_code,
      byte[] destination, Exception exception) {
      logger_.Error(exception_code.ToString(), exception,
        new Dictionary<string, string> {
          {"messageId", message_id.ToString()},
        });
      return true;
    }

    /// <inheritdoc/>
    public bool Send(byte[] id, int type, byte[] message, byte[] destination,
      string token) {
      logger_.Info("Send => id:" + id + ",type:" + type
        + ",token:" + token);
      return true;
    }

    /// <inheritdoc/>
    public bool SendError(byte[] message_id, int exception_code, string error,
      byte[] destination) {
      logger_.Error(exception_code + " " + error,
        new Dictionary<string, string> {
          {"messageId", message_id.ToString()},
        });
      return true;
    }

    /// <inheritdoc/>
    public bool SendError(byte[] message_id, int exception_code, string error,
      byte[] destination, Exception exception) {
      logger_.Error(exception_code + " " + error, exception,
        new Dictionary<string, string> {
          {"messageId", message_id.ToString()},
        });
      return true;
    }
  }
}