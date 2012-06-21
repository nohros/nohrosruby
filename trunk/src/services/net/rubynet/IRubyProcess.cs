using System;

namespace Nohros.Ruby
{
  /// <summary>
  /// <see cref="IRubyProcess"/> is an abstract of the main ruby process. It
  /// provides the method and properties of a ruby process.
  /// </summary>
  /// <remarks>
  /// The .NET implementation of the ruby service host could runs  as a
  /// ruby service node subprocess or as a stand-alone application.
  /// <see cref="IRubyProcess"/> provides an abstraction for this two type of
  /// process.
  /// </remarks>
  internal interface IRubyProcess
  {
    /// <summary>
    /// Runs the process.
    /// </summary>
    void Run();

    /// <summary>
    /// Runs the process using the given command line switches.
    /// </summary>
    /// <param name="command_line_string">
    /// The process command line string.
    /// </param>
    void Run(string command_line_string);

    /// <summary>
    /// Gets a <see cref="RubyMessageChannel"/> that can be used to receive/send
    /// messages from/to the ruby service node.
    /// </summary>
    IRubyMessageChannel RubyMessageChannel { get; }
  }
}
