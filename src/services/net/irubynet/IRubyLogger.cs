using System;
using System.Collections.Generic;
using Nohros.Logging;

namespace Nohros.Ruby
{
  /// <summary>
  /// Defines the methods and properties provided by the ruby logging
  /// infrastructure.
  /// </summary>
  public interface IRubyLogger : ILogger
  {
    void Debug(string message, IDictionary<string, string> categorization);

    void Debug(string message, Exception exception,
      IDictionary<string, string> categorization);

    void Error(string message, IDictionary<string, string> categorization);

    void Error(string message, Exception exception,
      IDictionary<string, string> categorization);

    void Fatal(string message, IDictionary<string, string> categorization);

    void Fatal(string message, Exception exception,
      IDictionary<string, string> categorization);

    void Info(string message, IDictionary<string, string> categorization);

    void Info(string message, Exception exception,
      IDictionary<string, string> categorization);

    void Warn(string message, IDictionary<string, string> categorization);

    void Warn(string message, Exception exception,
      IDictionary<string, string> categorization);
  }
}
