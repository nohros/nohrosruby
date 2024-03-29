﻿using System;

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
  internal interface IRubyProcess: IRubyMessagePacketListener
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
    /// <remarks>
    /// Legacy version of this app was used to start a single service without
    /// a shell. For compatbility with this legacy softwares this method allows
    /// a string to be specified as a argument, this string represents the list
    /// of arguments that is accepted by the start command. When supplied the
    /// shell will run and the specified service will be started; after that
    /// the shell will runs normally.
    /// </remarks>
    void Run(string command_line_string);

    void Exit();

    /// <summary>
    /// Gets a <see cref="ProcessMessageChannel"/> that can be used to receive/send
    /// messages from/to the ruby service node.
    /// </summary>
    //IRubyMessageChannel ProcessMessageChannel { get; }
  }
}
