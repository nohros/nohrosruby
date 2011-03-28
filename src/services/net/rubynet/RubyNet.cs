using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Threading;
using System.IO.Pipes;

using log4net;
using Nohros.Desktop;
using Nohros.Logging;

namespace Nohros.Ruby.Service.Net
{
    static class RubyNet
    {
        const string kVersion =
            @"
RubyNet Version 0.3.0
";

        internal const string kHeader = kVersion + @"
Copyright (c) 2010 Nohros Systems Inc.
All Rights Reserved.

Use of this software code is governed by a MIT license.

Runs a .NET assembly like a console application.
";
        const string kUsageCommon = @"
  -assembly      specifes the assembly to load and run. This value must be
                 an absolute path or a path relative to the base directory.

  -type          specifies the fully qualified type name of a class
                 that implements the IRubyService interface.

  -with-shell    specifies that the a command line language interpreter
                 must be started. With a shell users can stop and start
                 services directly from the command line and without
                 the intervention of a ruby net agent. It can to send
                 commands to a running service. Check out the full
                 documentation to know how to do it.

   ARGS          A list of arguments to pass to the loaded assembly. The
                 list of arguments must be preceded by a '-- ' argument
                 (without quotes).
";
        const string kUsage = kHeader + @"

Usage: nohros.rubynet -assembly=ASSEMBLYNAME -type=TYPENAME [-help] -- ARGS
"
            + kUsageCommon +
@"

  -help          Displays this help and exit.
  
  -version       Displays this help and exit.

   Examples:
     nohros.rubynet -assembly=my.assembly.dll -type=my.type, my.namespace
                    -- -debug -path=c:\\path\\

     The my.assembly will be loaded into the rubynet domain and a new
     instance of the my.type will be created. If the type instantiation is
     successful then the Run method will be called and the program control
     will be transfered to the loaded assembly.

     Do not attempt to start a nohros.rubynet program using the nohros.rubynet.
";

        const string kAssemblyNameSwitch = "assembly";
        const string kTypeNameSwitch = "type";
        const string kHelpSwitch = "help";
        const string kDebugSwitch = "debug";
        const string kWithShellSwitch = "with-shell";

        internal const string kPipeSwitch = "pipe";

        /// <summary>
        /// The main application logger.
        /// </summary>
        static ILog logger;

        static void Main() {
            logger = FileLogger.ForCurrentProcess.Logger;
            CommandLine command_line = CommandLine.ForCurrentProcess;
            RubyNetShell rn_shell = new RubyNetShell();

            if (command_line.HasSwitch(kDebugSwitch))
                System.Diagnostics.Debugger.Launch();

            bool with_shell = false;
            with_shell = command_line.HasSwitch(kWithShellSwitch);

            if (command_line.HasSwitch(kHelpSwitch))
                logger.Info(kUsage);

            // we cannot control the behavior of the service. For, logger purposes, we will monitor
            // all the unhandled application exception. NOTE: this does not prevents the application
            // from shutting down, its is used only for logging.
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(OnUnhandledException);

            // start a service if we have enough information to do it or explain to the caller how to use this
            // application.
            if (!(command_line.HasSwitch(kAssemblyNameSwitch) && command_line.HasSwitch(kTypeNameSwitch))) {
                if (!with_shell) {
                    logger.Info(kUsage);
                    return;
                }
            } else {
                rn_shell.StartAndExit(command_line);
            }

            // starts the shell if desired.
            if (with_shell)
                rn_shell.Start();
        }

        /// <summary>
        /// Logs all the application unhandled exceptions.
        /// </summary>
        /// <param name="sender">The source of the unhandled exception event.</param>
        /// <param name="e">An <see cref="UnhandledExceptionEventArgs"/> that contains the event data.</param>
        /// <remarks>The behavior of the service hosted by us, is not over our control. There is no way
        /// to prevents the application from shutting down, but we can log it. We do it here.</remarks>
        static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e) {
            // The ExceptionObject property og the UnhandledExceptionEventArgs class
            // is not an Excepition because it is posible to throw object in .NET that do not
            // derive from System.Exception. This is possible in some CLR based languages but
            // not C#. We can safe cast it to System.Exception.
            logger.Fatal("[Main   Nohros.Ruby.Service.Net.RubyNet]", (Exception)e.ExceptionObject);
        }

        /// <summary>
        /// Custom wrapper aroung logger.Error();
        /// </summary>
        internal static void LOG_ERROR(string message) {
            LOG_ERROR(message, null);
        }

        /// <summary>
        /// Custom wrapper aroung logger.Error();
        /// </summary>
        internal static void LOG_ERROR(string message, Exception exception) {
            logger.Error(message, exception);
        }
    }
}