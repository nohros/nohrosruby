using System;
using System.Collections.Generic;
using Nohros.Logging;

namespace Nohros.Ruby
{
  public class NopRubyLogger : NOPLogger, IRubyLogger
  {
    public void Debug(string message, IDictionary<string, string> categorization) {
    }

    public void Debug(string message, Exception exception,
      IDictionary<string, string> categorization) {
    }

    public void Error(string message, IDictionary<string, string> categorization) {
    }

    public void Error(string message, Exception exception,
      IDictionary<string, string> categorization) {
    }

    public void Fatal(string message, IDictionary<string, string> categorization) {
    }

    public void Fatal(string message, Exception exception,
      IDictionary<string, string> categorization) {
    }

    public void Info(string message, IDictionary<string, string> categorization) {
    }

    public void Info(string message, Exception exception,
      IDictionary<string, string> categorization) {
    }

    public void Warn(string message, IDictionary<string, string> categorization) {
    }

    public void Warn(string message, Exception exception,
      IDictionary<string, string> categorization) {
    }
  }
}
