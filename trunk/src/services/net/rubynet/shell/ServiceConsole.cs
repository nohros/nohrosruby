using System;
using Nohros.MyToolsPack.Console;

namespace Nohros.Ruby.Service.Net
{
  /// <summary>
  /// Implements the <see cref="IConsole"/> interface that runs when the
  /// application is in the service mode.
  /// </summary>
  /// <remarks>
  /// When running in service mode the <see cref="ReadLine"/> method will
  /// blocks forever and the <see cref="Write"/> and <see cref="WriteLine"/>
  /// methods will do nothing.
  /// </remarks>
  public class ServiceConsole : IConsole
  {
    /// <summary>
    /// Blocks until application exit.
    /// </summary>
    /// <returns></returns>
    public string ReadLine() {
      return Console.ReadLine();
    }

    /// <summary>
    /// Do no operation.
    /// </summary>
    public void Write(string message) { }

    /// <summary>
    /// Do no operation.
    /// </summary>
    public void WriteLine(string message) { }
  }
}
