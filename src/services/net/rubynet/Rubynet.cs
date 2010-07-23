using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

using log4net;
using Nohros.Desktop;

namespace Nohros.Ruby.Service.Net
{
    static class Rubynet
    {
        const string kUsage =
@"
RubyNet version 0.1.1
Copyright (c) 2010 Nohros Systems Inc.
All Rights Reserved.

Use of this software code is governed by a MIT license.

Runs a .NET assembly like a console application.

Usage: nohros.rubynet [-help] -assembly=ASSEMBLYNAME -type=TYPENAME [arguments]

   arguments     Any argument to pass to the loaded assembly.

  -assembly      specifes the assembly to load and run. This value must be
                 an absolute path or a path relative to the base directory.

  -type          specifies the fully qualified type name of a class
                 that implements the IRubyService interface.

  -help          Displays this help and exit.";

        const string kAssemblyNameSwitch = "assembly";
        const string kTypeNameSwitch = "type";
        const string kHelpSwitch = "help";

        static void Main() {
#if DEBUG
            ILog logger = LogManager.GetLogger("ConsoleLogger");
#else
            ILog logger = LogManager.GetLogger("FileLogger");
#endif
            CommandLine command_line = CommandLine.ForCurrentProcess;
            if (command_line.HasSwitch(kHelpSwitch) || !(command_line.HasSwitch(kAssemblyNameSwitch) && command_line.HasSwitch(kTypeNameSwitch))) {
                logger.Info(kUsage);
                return;
            }

            string assembly_path = command_line.GetSwitchValue(kAssemblyNameSwitch);
            string assembly_qualified_name = command_line.GetSwitchValue(kTypeNameSwitch);

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

            // all the command line loose values will be passed to the created assembly.
            IList<string> loose_values = command_line.LooseValues;

            int j = loose_values.Count;
            string[] args = new string[j];

            for (int i = 0; i < j; i++) {
                args[i] = loose_values[j];
            }

            try {
                IRubyService service = Activator.CreateInstance(sevice_type, args) as IRubyService;
                if (service == null) {
                    logger.Error("[Main   Nohros.Ruby.Service.Net.RubyNet]   An instance of the " + service + "type could not be created. Check the constructor implied by the IDevice interface.");
                    return;
                }

                service.Run();
            } catch(Exception ex) {
                logger.Fatal("[Main   Nohros.Ruby.Service.Net.RubyNet]", ex);
            }
        }
    }
}
