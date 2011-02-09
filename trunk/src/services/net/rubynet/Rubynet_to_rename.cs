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
        const string kUsage =
@"
RubyNet version 0.1.1
Copyright (c) 2010 Nohros Systems Inc.
All Rights Reserved.

Use of this software code is governed by a MIT license.

Runs a .NET assembly like a console application.

Usage: nohros.rubynet -assembly=ASSEMBLYNAME -type=TYPENAME [-help] -- ARGS

  -assembly      specifes the assembly to load and run. This value must be
                 an absolute path or a path relative to the base directory.

  -type          specifies the fully qualified type name of a class
                 that implements the IRubyService interface.

   ARGS          A list of arguments to pass to the loaded assembly. The
                 list of arguments must be preceded by a '-- ' argument
                 (without quotes).

  -help          Displays this help and exit.

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
        internal const string kPipeSwitch = "pipe";

        /// <summary>
        /// Synchronization object.
        /// </summary>
        static ManualResetEvent mutex = null;

        /// <summary>
        /// The main application logger.
        /// </summary>
        static ILog logger;

        static void Main() {
#if DEBUG
            logger = FileLogger.ForCurrentProcess.Logger;
#else
            logger = FileLogger.ForCurrentProcess.Logger;
#endif
            CommandLine command_line = CommandLine.ForCurrentProcess;
            if (command_line.HasSwitch(kDebugSwitch))
                System.Diagnostics.Debugger.Break();

            if (command_line.HasSwitch(kHelpSwitch) || !(command_line.HasSwitch(kAssemblyNameSwitch) && command_line.HasSwitch(kTypeNameSwitch))) {
                logger.Info(kUsage);
                return;
            }

            string assembly_path = command_line.GetSwitchValue(kAssemblyNameSwitch);
            string assembly_qualified_name = command_line.GetSwitchValue(kTypeNameSwitch);

            mutex = new ManualResetEvent(false);

            // if the path is not absolute it must be relative to the application base diractory.
            if (!Path.IsPathRooted(assembly_path))
                assembly_path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assembly_path);

            if (!File.Exists(assembly_path)) {
                logger.Error("[Main   Nohros.Ruby.Service.Net.RubyNet]   Could not find a assembly at the path:" + assembly_path);
                return;
            }

            Assembly service_assemlby = Assembly.LoadFrom(assembly_path);
            Type sevice_type = service_assemlby.GetType(assembly_qualified_name);
            if (sevice_type == null || sevice_type.GetInterface("Nohros.Ruby.IRubyService") == null) {
                logger.Error("[Main   Nohros.Ruby.Service.Net.RubyNet]   The type " + assembly_qualified_name + " could not be loaded or it does implements the [Nohros.Ruby.IRubyService interface].");
                return;
            }

            // builds a command line to pass to the assembly.
            string assembly_command_line = assembly_path;

            IList<string> loose_values = command_line.LooseValues;
            for (int i = 0, j = loose_values.Count; i < j; i++) {
                assembly_command_line += string.Concat(" ", loose_values[i]);
            }

            try {
                IRubyService service = Activator.CreateInstance(sevice_type, assembly_command_line) as IRubyService;
                if (service == null) {
                    logger.Error("[Main   Nohros.Ruby.Service.Net.RubyNet]   An instance of the " + service + "type could not be created. Check the constructor implied by the IDevice interface.");
                    return;
                }

                // we cannot control the behavior of the service. For, logger purposes, we will monitor
                // all the unhandled application exception. NOTE: this does not prevents the application
                // from shutting down, its is used only for logging.
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(OnUnhandledException);

                // attempt to get the IPC channel name.
                string pipe_name = command_line.GetSwitchValue(kPipeSwitch);

                // host and start the service.
                RubyNetServiceHost host = new RubyNetServiceHost(service, pipe_name);
                host.StartService();

            } catch(Exception ex) {
                Exception exception = ex;
                do {
                    logger.Fatal("[Main   Nohros.Ruby.Service.Net.RubyNet]", exception);
                    exception = exception.InnerException;
                } while (exception != null);
            }
        }

        internal static void LOG_ERROR(string message) {
            LOG_ERROR(message, null);
        }

        internal static void LOG_ERROR(string message, Exception exception) {
            logger.Error(message, exception);
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
    }
}