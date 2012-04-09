using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MyToolsPack.Console;

namespace Nohros.Ruby.Service.Net
{
  /// <summary>
  /// Implements the <see cref="IConsole"/> interface that runs when the
  /// application is in the service mode.
  /// </summary>
  /// <remarks>
  /// When running in service mode the ReadXXX methods will return the
  /// server sent messages and the WriteXXX methods will write the messages to
  /// the current application logger using the INFO level.
  /// </remarks>
  public class ServiceConsole : IConsole
  {
    #region .ctor
    /// <summary>
    /// Initialize a new instance of the <see cref="ServiceConsole"/> class.
    /// </summary>
    public ServiceConsole() { }
    #endregion

    // TODO: read messages from the server.
    public string ReadLine() {
      return System.Console.ReadLine();
    }

    /// <summary>
    /// Writes the specified message to the current application logger using
    /// the <see cref="LogLevel.INFO"/>
    /// </summary>
    /// <param name="message">The message to write.</param>
    public void Write(string message) {
      RubyLogger.ForCurrentProcess.Info(message);
    }

    /// <summary>
    /// Writes the specified message, followed by the current line terminator,
    /// to the current application logger using the <see cref="LogLevel.INFO"/>
    /// </summary>
    /// <param name="message">The message to write.</param>
    public void WriteLine(string message) {
      RubyLogger.ForCurrentProcess.Info(message + Environment.NewLine);
    }
  }
}
