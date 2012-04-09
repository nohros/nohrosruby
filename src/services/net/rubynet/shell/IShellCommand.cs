using System;
using System.Collections.Generic;
using System.Text;

using log4net;

using Nohros;
using Nohros.Desktop;

namespace Nohros.Ruby.Service.Net
{
    /// <summary>
    /// Defines the methods and properties of the commands that are recognized by the ruby net shell.
    /// </summary>
    /// <remarks>This interface implies a constructor with the following signature:
    /// <para>
    /// IShellCommand(Nohros.Desktop.CommandLine command_line, RubyNetShell shell).
    /// where |comman_line| parameter represents the command line string that was typed into the shell and
    /// the |shell| command represents the current ruby net shell object.
    /// </para>
    /// </remarks>
    internal interface IShellCommand
    {
        /// <summary>
        /// The name of the command.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Parse the command, validate the specified list of arguments and execute it if posible.
        /// </summary>
        void Execute();
    }
}
