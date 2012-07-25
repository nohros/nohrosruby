using System;

namespace Nohros.Ruby
{
  internal sealed class Strings
  {
    /// <summary>
    /// The name of node that contains the logging provider configuration.
    /// </summary>
    public const string kLoggingProviderNode = "LoggingProvider";

    /// <summary>
    /// The name of the node that contains the configuration data for the
    /// MyToolsPack library.
    /// </summary>
    public const string kMyToolsPackNode = "mytoolspack";

    /// <summary>
    /// The address of th IPC channel tha handles the communication between
    /// the services and clients.
    /// </summary>
    public const string kIPCChannelAddress = "ipc-channel-address";

    /// <summary>
    /// The zeromq socket's endpoint that send messages to services.
    /// </summary>
    public const string kMessageSenderEndpoint = "message-sender-endpoint";

    public const string kRubyHostServiceName = "ruby-service-host";

    public const string kNodeServiceName = "nohros.ruby.node";

    public const string kHostServiceFact = "host";
    public const string kServiceAssembly = "assembly";
    public const string kServiceName = "name";
    public const string kServiceType = "type";
    public const string kServiceSwitches = "switches";

    /// <summary>
    /// The name of the fodler that is used to store the services binaries.
    /// </summary>
    public const string kServicesFolderName = "services";

    public const string kRunningMode = "running-mode";

    /// <summary>
    /// The string that is displayed on the shell prompt.
    /// </summary>
    public const string kShellPrompt = "rubynet$: ";

    public const string kHelp = "help";
    public const string kWaitForDebugger = "debug";

    public const string kVersion = @"
RubyNet Version 0.3.0
";

    public const string kHeader =
      kVersion +
        @"
Copyright (c) 2010 Nohros Systems Inc. All Rights Reserved.

Use of this software code is governed by a MIT license.
";

    public const string kUsageCommon =
      @"
  -assembly      specifes the assembly to load and run. This value must be
                 an absolute path or a path relative to the base directory.

  -type          specifies the fully qualified type name of a class
                 that implements the IRubyService interface.

   ARGS          A list of arguments to pass to the loaded assembly. The
                 list of arguments must be preceded by a '-- ' argument
                 (without quotes).
";

    public const string kUsage =
      kHeader +
        @"
Runs a .NET assembly like a console application.
Usage: nohros.rubynet -assembly=ASSEMBLYNAME -type=TYPENAME [-help] -- ARGS
"
          + kUsageCommon +
            @"
  -with-shell    specifies that the a command line language interpreter
                 must be started. With a shell users can stop and start
                 services directly from the command line and without
                 the intervention of a ruby net agent. It can to send
                 commands to a running service. Check out the full
                 documentation to know how to do it.

  -help          Displays this help and exit.
  
  -version       Displays the version and exit.

   Examples:
     nohros.rubynet -assembly=my.assembly.dll -type=my.type, my.namespace
                    -- -debug -path=c:\\path\\

     The my.assembly will be loaded into the rubynet domain and a new
     instance of the my.type will be created. If the type instantiation is
     successful then the Run method will be called and the program control
     will be transfered to the loaded assembly.

     Do not attempt to start a nohros.rubynet program using the nohros.rubynet.
";
  }
}
