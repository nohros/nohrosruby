﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.454
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Nohros.Ruby {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Nohros.Ruby.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The {0} was not specified or is invalid..
        /// </summary>
        internal static string Arg_MissingOrInvalid {
            get {
                return ResourceManager.GetString("Arg_MissingOrInvalid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot perform this operation on a closed channel..
        /// </summary>
        internal static string InvalidOperation_ClosedChannel {
            get {
                return ResourceManager.GetString("InvalidOperation_ClosedChannel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not find a assembly at the path: {0}.
        /// </summary>
        internal static string log_assembly_not_found {
            get {
                return ResourceManager.GetString("log_assembly_not_found", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The console could not be created..
        /// </summary>
        internal static string log_console_creation_failed {
            get {
                return ResourceManager.GetString("log_console_creation_failed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An instance of the {0} type could not be created. Check the constructor implied by the IRubyServiceFactory interface..
        /// </summary>
        internal static string log_irubyservice_factory_constructor_error {
            get {
                return ResourceManager.GetString("log_irubyservice_factory_constructor_error", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An instance of the service could not be created using the {0} factory..
        /// </summary>
        internal static string log_irubyservice_load_error {
            get {
                return ResourceManager.GetString("log_irubyservice_load_error", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The system has rached the maximum number of threads allowed to run simultaneously..
        /// </summary>
        internal static string log_max_number_services {
            get {
                return ResourceManager.GetString("log_max_number_services", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A pipe named {0} already exists..
        /// </summary>
        internal static string log_pipe_already_exists {
            get {
                return ResourceManager.GetString("log_pipe_already_exists", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The service {0} is not running..
        /// </summary>
        internal static string log_service_not_running {
            get {
                return ResourceManager.GetString("log_service_not_running", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to One of the required arguments was not supplied..
        /// </summary>
        internal static string log_shell_start_required_args {
            get {
                return ResourceManager.GetString("log_shell_start_required_args", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to load the type:{0}.
        /// </summary>
        internal static string log_type_load_failed {
            get {
                return ResourceManager.GetString("log_type_load_failed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The type of socket {0} should be {1}..
        /// </summary>
        internal static string log_zmq_socket_is_not_of_type {
            get {
                return ResourceManager.GetString("log_zmq_socket_is_not_of_type", resourceCulture);
            }
        }
    }
}
