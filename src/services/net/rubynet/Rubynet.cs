using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using log4net;

namespace Nohros.Ruby.Service.Net
{
    static class Rubynet
    {
        static void Main(string[] args) {
#if DEBUG
            ILog logger = LogManager.GetLogger("DebugLogger");
#else
            ILog logger = LogManager.GetLogger("ReleaseLogger");
#endif

            int args_length = args.Length;
            if (args_length < 2) {
                logger.Error("Invalid number of arguments");
                return;
            }

            string assembly_path = args[0];
            string assembly_qualified_name = args[1];

            string[] new_args;
            if (args_length > 1) {
                new_args = new string[args_length - 1];
                for (int i = 2, j = 0; i < args_length; i++, j++) {
                    new_args[j] = args[i];
                }
            } else {
                new_args = new string[0];
            }

            int dir_separator_pos = assembly_path.IndexOf(':');
            if (dir_separator_pos == -1) {
                assembly_path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assembly_path);
            }

            if (!File.Exists(assembly_path)) {
                logger.Error("Could not find a assembly at the path:" + assembly_path);
                return;
            }

            Assembly service_assemlby = Assembly.LoadFrom(assembly_path);
            Type sevice_type = service_assemlby.GetType(assembly_qualified_name);
            if (sevice_type == null || sevice_type.GetInterface("Nohros.Ruby.IRubyService") == null) {
                logger.Error("The type " + assembly_qualified_name + " could not be loaded or it does implements the [Nohros.Ruby.IRubyService interface].");
#if DEBUG
                System.Diagnostics.Debugger.Break();
#endif
                return;
            }

            try {
                IRubyService service = Activator.CreateInstance(sevice_type, new_args) as IRubyService;
                if (service == null) {
                    logger.Error("An instance of the " + service + "type could not be created. Check the constructor implied by the IDevice interface.");
                    return;
                }

                service.Run();
            } catch(Exception ex) {
                logger.Fatal(ex.Message, ex);
            }
        }
    }
}
