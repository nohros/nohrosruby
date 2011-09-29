using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using System.Collections.Specialized;
using System.Threading;

using Nohros.Logging;
using Nohros.Desktop;

namespace Nohros.Ruby.Service.Net
{
    internal class RubyShell
    {
        const string kAssemblyNameSwitch = "assembly";
        const string kTypeNameSwitch = "type";

        const string kShellPrompt = "rubynet$: ";
        const string kExitCommand = "exit";
        const int kMaxThreads = 10;

        const string kStartCommandName = "start";
        const string kStopCommandName = "stop";
        const string kSendCommandName = "send";
        const string kHelpCommandName = "help";
        const string kExitCommandName = "exit";
        const string kClearCommandName = "clear";

        ListDictionary hosted_services_threads_;

        #region ServiceListKey
        struct ServiceListKey {
            Thread thread_;
            IRubyService service_;
            bool is_valid_;

            public static ServiceListKey INVALID;

            #region .ctor
            static ServiceListKey() {
                INVALID = new ServiceListKey(null, null);
            }

            /// <summary>
            /// Initializes a new instance of the calss <see cref="ServiceListKey"/> by using the specified
            /// related thread and service.
            /// </summary>
            /// <param name="thread">The thread where the service is running.</param>
            /// <param name="service">The service related with this key.</param>
            public ServiceListKey(Thread thread, IRubyService service) {
                is_valid_ = (thread != null || service != null);
                
                // if the key is invalid all its members must be equals to null.
                if (is_valid_) {
                    thread_ = thread;
                    service_ = service;
                } else {
                    thread_ = null;
                    service_ = null;
                }
            }
            #endregion

            /// <summary>
            /// Gets the hash code for this instance.
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode() {
                // Since each service runs into a dedicated thread, the hash code of
                // this thread could be used like the hash code of the key.
                return thread_.GetHashCode();
            }

            /// <summary>
            /// Indicates whether this instance and a specified object are equal.
            /// </summary>
            /// <param name="obj">Another object to compare.</param>
            /// <returns>true if <paramref name="obj"/> and this instance are the same type and represents
            /// the same value; otherwise, false.</returns>
            public override bool Equals(object obj) {
                // MSDN: http://msdn.microsoft.com/en-us/library/ms173147(v=vs.80).aspx
                // x.Equals(null) should returns false.
                return (obj != null && obj is ServiceListKey && ((ServiceListKey)(obj)).thread_ == thread_);
            }

            /// <summary>
            /// Gets a value indicating whether two <see cref="ServiceListKey"/> are equals.
            /// </summary>
            /// <param name="keyA">The first object to compare.</param>
            /// <param name="keyB">The second object to compare.</param>
            /// <returns>true</returns>
            public static bool operator==(ServiceListKey keyA, ServiceListKey keyB) {
                return (keyA.thread_ == keyB.thread_);
            }

            public static bool operator!=(ServiceListKey keyA, ServiceListKey keyB) {
                return (keyA.thread_ != keyB.thread_);
            }

            /// <summary>
            /// Gets a value indicating wheter the key is valid.
            /// </summary>
            /// <returns>true if key is valid; otherwise, false.</returns>
            /// <remarks>
            /// A invalid key has its members and properties equals to null.
            /// </remarks>
            public bool IsValid() {
                return is_valid_;
            }

            /// <summary>
            /// The thread where the service is running.
            /// </summary>
            public Thread Thread {
                get { return thread_; }
            }

            /// <summary>
            /// Gets the service object related with this key.
            /// </summary>
            public IRubyService Service {
                get { return service_; }
            }
        }
        #endregion

        #region .ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="RubyNetShell"/>.
        /// </summary>
        public RubyNetShell() {
            // TODO: get the logger object using a factory based on the app configuration file.
            console_logger_ = new Log4NetLogger(ConsoleLogger.ForCurrentProcess.Logger);
            hosted_services_threads_ = new ListDictionary();
        }
        #endregion

        /// <summary>
        /// Starts a service and terminates the application when the service finish.
        /// </summary>
        /// <param name="command_line"></param>
        public void RunAndExit(CommandLine command_line) {
            try {
                StartCommand command = StartCommand.FromCommandLine(command_line);
                StartService(command);
            } catch (Exception exception) {
                console_logger_.Error("[Nohros.Ruby.Service.Net.RunAndExit]", exception);
            }
        }

        /// <summary>
        /// Starts the command line shell.
        /// </summary>
        /// <param name="initial_start_command">A string representing the parameters of a start command.</param>
        /// <remarks>
        /// Legacy version of this app was used to start a single service without a shell. For compatbility with
        /// this legacy softwares this method allows a string to be specified as a argument, this string represents
        /// the list of arguments that is accepted by the start command. When supplied the shell will run and
        /// the specified service will be started; after that the shell will runs normally.
        /// </remarks>
        public void Run(string initial_start_command) {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;

            CommandType type = CommandType.Unknown;

            if (initial_start_command != null && initial_start_command.Length > 0) {
                try {
                    StartCommand command = StartCommand.FromCommandLine(CommandLine.FromString(initial_start_command));
                    StartService(command);
                } catch(Exception exception) {
                    console_logger_.Error("[Nohros.Ruby.Service.Net.Run@initial_start_command]", exception);
                }
            }

            do {
                Console.Write(kShellPrompt);

                CommandLine next_command_line = null;
                string next_command = string.Empty
                    ,next_command_line_string = Console.ReadLine();

                // the first sequence of characters followed by a space is the command name
                // everything else is the command parameters.
                int index = next_command_line_string.IndexOf(" ");
                if (index != -1) {
                    next_command = next_command_line_string.Substring(0, index);
                } else {
                    next_command = next_command_line_string;
                }

                if (next_command == string.Empty)
                    continue;

                next_command_line = CommandLine.FromString(next_command_line_string);
                type = ParseCommand(next_command.ToLower());

                try {
                    switch (type) {
                        case CommandType.Start:
                            StartCommand start_command = StartCommand.FromCommandLine(next_command_line);
                            StartService(start_command);
                            break;

                        case CommandType.Stop:
                            StopCommand stop_command = StopCommand.FromCommandLine(next_command_line);
                            StopService(stop_command);
                            break;

                        case CommandType.Send:
                            break;

                        case CommandType.Help:
                            string help_command_ref =
                                (next_command_line_string.Length != next_command.Length) ?
                                 next_command_line_string.Substring(next_command.Length).Trim() : next_command_line_string;
                            DisplayHelp(help_command_ref);
                            break;

                        case CommandType.Exit:
                            break;

                        case CommandType.Clear:
                            Console.Clear();
                            break;

                        case CommandType.Unknown:
                            console_logger_.Warn("The " + next_command + " is not a recognized command.");
                            break;
                    }
                } catch(Exception exception) {
                    console_logger_.Error("[Nohros.Ruby.Service.Net.Run]", exception);
                }
            } while (type != CommandType.Exit);
        }

        /// <summary>
        /// Parses a command.
        /// </summary>
        /// <param name="lowercase_command">The lowercase version of the command name.</param>
        /// <returns>The type of the command.</returns>
        CommandType ParseCommand(string lowercase_command) {
            char c = lowercase_command[0];
            switch(c) {
                case 's':
                    c = lowercase_command[1];
                    if(lowercase_command[1] == 't') {
                        if(lowercase_command[2] == 'a' && NextStringMatch(lowercase_command, kStartCommandName)) {
                            return CommandType.Start;
                        } else if (lowercase_command[2] == 'o' && NextStringMatch(lowercase_command, kStopCommandName)) {
                            return CommandType.Stop;
                        }
                    }
                    else if(lowercase_command[1] == 'e' && NextStringMatch(lowercase_command, kSendCommandName)) {
                        return CommandType.Send;
                    }
                    break;

                case 'e':
                    if (NextStringMatch(lowercase_command, kExitCommand))
                        return CommandType.Exit;
                    break;

                case 'h':
                    if (NextStringMatch(lowercase_command, kHelpCommandName)) {
                        return CommandType.Help;
                    }
                    break;

                case 'c':
                    if (NextStringMatch(lowercase_command, kClearCommandName)) {
                        return CommandType.Clear;
                    }
                    break;

                default:
                    return CommandType.Unknown;
            }
            return CommandType.Unknown;
        }

        /// <summary>
        /// Checks if a given string matches another string.
        /// </summary>
        /// <param name="str">The string to compare.</param>
        /// <param name="match">The string to match.</param>
        /// <returns></returns>
        bool NextStringMatch(string str, string match) {
            if (str.Length != match.Length)
                return false;

            for (int i = 0, j = str.Length; i < j; i++) {
                if (str[i] != match[i])
                    return false;
            }
            return true;
        }

        #region Commands
        /// <summary>
        /// Executes the specified start service command.
        /// </summary>
        /// <param name="command">The service command containing the</param>
        /// <remarks>
        /// The start command will start a new service into a dedicated thread. The dedicated thread
        /// will be a foreground thread, so we can force it to shutdown and avoid application hangs.
        /// </remarks>
        public void StartService(StartCommand command) {
            if (hosted_services_threads_.Count > kMaxThreads)
                console_logger_.Warn(Resources.log_max_number_services);

            object[] service_thread_parms = new object[2] { command, console_logger_ };

            Thread service_thread = new Thread(new ParameterizedThreadStart(
                delegate(object o) {
                    object[] parms = (object[])o;

                    StartCommand cmd = parms[0] as StartCommand;
                    ILog logger = parms[1] as ILog;
                    IRubyServiceHost host = cmd.StartService(true, logger);
                    string service_name = host.Service.Name;

                    // Accordinly to the MSDN, there is no such thing as an unhandled exception on a thread
                    // created with the |Run| method of the |Thread| class. When code running on such a
                    // thread throws an exception that it does not handle, the runtime prints the exception
                    // stack trace to the console and then gracefully terminates the thread. So we need to
                    // handle the excetpions to avoid keep an invalid thread into our list of running services
                    // threads.
                    try {
                        // we need to add the service thread to the list of running services
                        // before the service starts, because the service blocks the current
                        // thread it finish you work.
                        hosted_services_threads_.Add(service_name, Thread.CurrentThread);

                        #region debugging
                        if (logger.IsDebugEnabled)
                            logger.Debug("[StartService   Nohros.Ruby.Service.Net]   The service " + service_name + " has been started.");
                        #endregion

                        host.StartService();
                    } catch(Exception e) {
                        // invalid services must be removed from the list of running services.
                        hosted_services_threads_.Remove(service_name);
                        logger.Error("[StartService   Nohros.Ruby.Service.Net]", e);
                    }

                    // the service has been finished your work, we can remove it from the
                    // list of running services.
                    hosted_services_threads_.Remove(service_name);

                    #region debugging
                    if (logger.IsDebugEnabled)
                        logger.Debug("[StartService   Nohros.Ruby.Service.Net]   The service " + service_name + " has been finished.");
                    #endregion
                }
            ));

            service_thread.IsBackground = true;
            service_thread.Start(service_thread_parms);
        }

        /// <summary>
        /// Stops a service execution.
        /// </summary>
        public void StopService(StopCommand command) {
            string service_name = command.ServiceName;
            object service_key_object = hosted_services_threads_[service_name];
            if(service_key_object == null)
                console_logger_.Warn(Resources.log_shell_stop_service_not_found);

            // executes the stop method in a thread from the thread pool and
            // waits the thread to finish its execution. Note that the
            // service thread will be forced to kill 30 seconds after
            // the stop method starts.
            ThreadPool.QueueUserWorkItem(delegate(object service_list_key) {
                ServiceListKey service = (ServiceListKey)service_list_key;
                service.Service.Stop();
            }, service_key_object);

            bool((ServiceListKey)service_key_object).Thread.Join(3000);
        }

        public void DisplayHelp(string command_ref) {
            HelpCommand help_command = new HelpCommand(ParseCommand(command_ref.ToLower()));
            console_logger_.Info(help_command.GetHelpText());
        }
        #endregion
    }
}