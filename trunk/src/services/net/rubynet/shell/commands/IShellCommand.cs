using System;
using System.Collections.Generic;
using System.Text;

using Nohros.Desktop;

namespace Nohros.Ruby.Service.Net
{
  /// <summary>
  /// Defines a interface that all ruby shell commands must follow
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
    /// <param name="shell">The <see cref="RubyShell"/> object that is related
    /// with the command.</param>
    void Run(RubyShell shell);
  }
}