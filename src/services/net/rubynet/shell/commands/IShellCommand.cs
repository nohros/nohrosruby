using System;

using Nohros.Desktop;

namespace Nohros.Ruby.Shell
{
  /// <summary>
  /// Defines a interface that all ruby process commands must follow
  /// </summary>
  internal interface IShellCommand
  {
    /// <summary>
    /// Gets the name of the command.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets or sets <see cref="CommandLine"/> object that represents the
    /// command switches.
    /// </summary>
    /// <seealso cref="CommandLine"/>
    CommandLine Switches { get; set; }

    /// <summary>
    /// Runs the commmand.
    /// </summary>
    /// <param name="process">The <see cref="ShellRubyProcess"/> object that is related
    /// with the command.</param>
    void Run(ShellRubyProcess process);
  }
}