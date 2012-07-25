using System;

namespace Nohros.Ruby
{
  /// <summary>
  /// An implementation of the <see cref="IRubyLogger"/> interface that is
  /// disabled for all levels.
  /// </summary>
  public class NoOpLogger : IRubyLogger
  {
    /// <inheritdoc/>
    public bool IsDebugEnabled {
      get { return false; }
    }

    /// <inheritdoc/>
    public bool IsFatalEnabled {
      get { return false; }
    }

    /// <inheritdoc/>
    public bool IsInfoEnabled {
      get { return false; }
    }

    /// <inheritdoc/>
    public bool IsErrorEnabled {
      get { return false; }
    }

    /// <inheritdoc/>
    public bool IsWarnEnabled {
      get { return false; }
    }

    /// <inheritdoc/>
    public bool IsTraceEnabled {
      get { return false; }
    }

    /// <inheritdoc/>
    public void Debug(string message) {
    }

    /// <inheritdoc/>
    public void Debug(string message, Exception exception) {
    }

    /// <inheritdoc/>
    public void Error(string message) {
    }

    /// <inheritdoc/>
    public void Error(string message, Exception exception) {
    }

    /// <inheritdoc/>
    public void Fatal(string message) {
    }

    /// <inheritdoc/>
    public void Fatal(string message, Exception exception) {
    }

    /// <inheritdoc/>
    public void Info(string message) {
    }

    /// <inheritdoc/>
    public void Info(string message, Exception exception) {
    }

    /// <inheritdoc/>
    public void Warn(string message) {
    }

    /// <inheritdoc/>
    public void Warn(string message, Exception exception) {
    }
  }
}
