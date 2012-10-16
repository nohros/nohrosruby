using System;
using System.Collections.Generic;
using System.Text;
using Nohros.Logging;

namespace Nohros.Ruby
{
  /// <summary>
  /// A implementation of the <see cref="IRubyLogger"/> that forwards its
  /// methods to a <see cref="ILogger"/> object.
  /// </summary>
  public class LoggerRubyLogger : ForwardingLogger, IRubyLogger
  {
    readonly ILogger logger_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="RubyLogger"/>
    /// class by using the specified <see cref="ILogger"/> interface.
    /// </summary>
    public LoggerRubyLogger(ILogger logger)
      : base(logger) {
      logger_ = logger;
    }
    #endregion

    /// <inheritdoc/>
    public void Debug(string message, IDictionary<string, string> categorization) {
      logger_.Debug(GetMessage(message, categorization));
    }

    /// <inheritdoc/>
    public void Debug(string message, Exception exception,
      IDictionary<string, string> categorization) {
      logger_.Debug(GetMessage(message, categorization), exception);
    }

    /// <inheritdoc/>
    public void Error(string message, IDictionary<string, string> categorization) {
      logger_.Error(GetMessage(message, categorization));
    }

    /// <inheritdoc/>
    public void Error(string message, Exception exception,
      IDictionary<string, string> categorization) {
      logger_.Error(GetMessage(message, categorization), exception);
    }

    /// <inheritdoc/>
    public void Fatal(string message, IDictionary<string, string> categorization) {
      logger_.Fatal(GetMessage(message, categorization));
    }

    /// <inheritdoc/>
    public void Fatal(string message, Exception exception,
      IDictionary<string, string> categorization) {
      logger_.Fatal(GetMessage(message, categorization), exception);
    }

    /// <inheritdoc/>
    public void Info(string message, IDictionary<string, string> categorization) {
      logger_.Info(GetMessage(message, categorization));
    }

    /// <inheritdoc/>
    public void Info(string message, Exception exception,
      IDictionary<string, string> categorization) {
      logger_.Info(GetMessage(message, categorization), exception);
    }

    /// <inheritdoc/>
    public void Warn(string message, IDictionary<string, string> categorization) {
      logger_.Warn(GetMessage(message, categorization));
    }

    /// <inheritdoc/>
    public void Warn(string message, Exception exception,
      IDictionary<string, string> categorization) {
      logger_.Warn(GetMessage(message, categorization), exception);
    }

    string GetMessage(string message, IDictionary<string, string> categorization) {
      if (categorization.Count > 0) {
        StringBuilder builder = new StringBuilder(message);
        builder.Append(" Categorization =>");
        foreach (KeyValuePair<string, string> pair in categorization) {
          builder
            .Append(pair.Key)
            .Append(":")
            .Append(pair.Value);
        }
        return builder.ToString();
      }
      return message;
    }
  }
}
