using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Pipes;

using NUnit.Framework;
using log4net;

using Nohros.Logging;
using Nohros.Desktop;
using Nohros.Ruby.Service.Net;
using Nohros.Ruby;

namespace Nohros.Ruby.Tests.Net
{
    [TestFixture]
    [Category("ShellCommands")]
    public class StartCommand_
    {
        ILog logger;
        CommandLine command_line;

        const string kTestServiceAssembly = "nohros.ruby.tests.net.dll";

        const string kStartAndStopServiceType = "Nohros.Ruby.Tests.Net.RunAndStopService";
        const string kThrowExceptionWhenRunServiceType = "Nohros.Ruby.Tests.Net.ThrowExceptionWhenRunService";
        const string kNonRubyServiceType = "Nohros.Ruby.Tests.Net.NonRubyService";
        const string kWaitServiceAssembly = "Nohros.Ruby.Tests.Net.WaitService";

        [SetUp]
        public void SetUp() {
            logger = ConsoleLogger.ForCurrentProcess.Logger;
            command_line = CommandLine.ForCurrentProcess;
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowArgumentNullExceptionWhenCommandLineIsNull() {
            StartCommand command = new StartCommand(null, logger);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowArgumentNullExceptionWhenLoggerIsNull() {
            StartCommand command = new StartCommand(command_line, null);
        }

        [Test]
        public void ShouldConstructNewStartCommand() {
            StartCommand command = new StartCommand(command_line, logger);
        }

        [Test]
        public void ShouldReturnTheWordStartAsAString() {
            StartCommand command = new StartCommand(command_line, logger);
            Assert.AreEqual(command.Name, "start");
        }

        [Test]
        public void ShouldReturnEmptyServiceHostWhenServiceThrowExceptionOnRun() {
            StartCommand command = new StartCommand(command_line, logger);
            IRubyServiceHost host = command.StartService(kTestServiceAssembly, kThrowExceptionWhenRunServiceType, null, null);
            Assert.IsInstanceOf<EmptyServiceHost>(host);
        }

        [Test]
        public void ShouldReturnEmptyServiceHostWhenAssemblyDoesNotExixts() {
            StartCommand command = new StartCommand(command_line, logger);
            IRubyServiceHost host = command.StartService("missing-assembly-file", kStartAndStopServiceType, null, null);
            Assert.IsInstanceOf<EmptyServiceHost>(host);
        }

        [Test]
        public void ShouldReturnEmptyServiceHostWhenTypeCouldNotBeLoaded() {
            StartCommand command = new StartCommand(command_line, logger);
            IRubyServiceHost host = command.StartService(kTestServiceAssembly, "wrong-type", null, null);
            Assert.IsInstanceOf<EmptyServiceHost>(host);
        }

        [Test]
        public void ShouldReturnEmptyServiceHostWhenTypeDoesNotImplementsIRubyServiceInterface() {
            StartCommand command = new StartCommand(command_line, logger);
            IRubyServiceHost host = command.StartService(kTestServiceAssembly, "wrong-type", null, null);
            Assert.IsInstanceOf<EmptyServiceHost>(host);
        }

        [Test]
        public void ShouldStartTheServiceEvenWhenPipeChannelNameIsNull() {
            StartCommand command = new StartCommand(command_line, logger);
            IRubyServiceHost host = command.StartService(kTestServiceAssembly, kStartAndStopServiceType, null, string.Empty);
            Assert.IsNotInstanceOf<EmptyServiceHost>(host);
        }

        [Test]
        public void ShouldStartTheServiceEvenWhenCommandLineIsNull() {
            StartCommand command = new StartCommand(command_line, logger);
            IRubyServiceHost host = command.StartService(kTestServiceAssembly, kStartAndStopServiceType, string.Empty, null);
            Assert.IsNotInstanceOf<EmptyServiceHost>(host);
        }
    }
}
