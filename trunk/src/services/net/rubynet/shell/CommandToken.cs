using System;
using System.Collections.Generic;
using System.Text;

namespace Nohros.Ruby.Service.Net
{
    internal struct CommandToken
    {
        CommandType type_;
        IShellCommand command_;

        static CommandToken unknown_;

        #region .ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandType"/> by using the specified
        /// command type.
        /// </summary>
        /// <param name="type">The type of the command.</param>
        /// <remarks>
        /// By using this constructor the user should set the value of the command property explicit. Otherwise,
        /// attempt to get the value of the <see cref="Command"/> property returns null.
        /// </remarks>
        public CommandToken(CommandType type) {
            type_ = type;
            command_ = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandType"/> by using the specified
        /// command type and command.
        /// </summary>
        /// <param name="type">The type of the command.</param>
        /// <param name="command">A <see cref="IShellCommand"/> object representing the command.</param>
        public CommandToken(CommandType type, IShellCommand command) {
            type_ = type;
            command_ = command;
        }
        #endregion

        /// <summary>
        /// Gets the type of the command.
        /// </summary>
        public CommandType Type {
            get { return type_; }
            set { type_ = value; }
        }

        /// <summary>
        /// Gets a <see cref="IShellCommand"/> object representing a command whose type is <see cref="Type"/> or
        /// null if eithrt the value of this property was not set explicit or the if the appropiated constructor was not
        /// called.
        /// </summary>
        public IShellCommand Command {
            get { return command_; }
            set { command_ = value; }
        }
    }
}
