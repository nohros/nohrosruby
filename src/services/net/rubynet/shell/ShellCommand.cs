using System;
using System.Collections.Generic;
using System.Text;
using log4net;

using Nohros.Desktop;
using Nohros.Resources;

namespace Nohros.Ruby.Service.Net
{
    /// <summary>
    /// This class provides the common implementation of the <see cref="IShellCommand"/> interface and
    /// is intended to be used like a base class for classes that implements the <see cref="IShellCommand"/>.
    /// </summary>
    internal abstract class ShellCommand : IShellCommand
    {
        /// <summary>
        /// The command name.
        /// </summary>
        protected string name_;

        /// <summary>
        /// A <see cref="CommandLine"/> object representing the command line string that was typed
        /// into the shell.
        /// </summary>
        protected CommandLine command_line_;

        /// <summary>
        /// A ILog object that can be used to log messages.
        /// </summary>
        protected ILog logger_;

        #region .ctor
        /// <summary>
        /// Initializes a nes instance of the <see cref="ShellCommand"/> by using the specified command name
        /// and command line.
        /// </summary>
        /// <param name="name">The name of the command.</param>
        /// <param name="command_line">A <see cref="CommandLine"/> object representing the command line string
        /// that was typed into the shell.</param>
        /// <param name="logger">An <see cref="ILog"/> object that can be used to log errors.</param>
        /// <seealso cref="CommandLine"/>
        /// <exception cref="ArgumentNullException">one of the parameters is a null reference.</exception>
        /// <exception cref="ArgumentException"><paramref name="name"/> is a empty string.</exception>
        /// <seealso cref="CommandLine"/>
        public ShellCommand(string name, CommandLine command_line, ILog logger) {
            if (name.Length == 0)
                throw new ArgumentException("name");

            if (name == null || command_line == null || logger == null)
                throw new ArgumentNullException(StringResources.Argument_any_null);

            name_ = name;
            command_line_ = command_line;
            logger_ = logger;
        }
        #endregion

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <remarks>If the command is not already parsed the <see cref="Parse()"/> method
        /// will be called to parse it.</remarks>
        public abstract void Execute();

        /// <summary>
        /// Gets the commmand name.
        /// </summary>
        public string Name {
            get { return name_; }
        }
    }
}
