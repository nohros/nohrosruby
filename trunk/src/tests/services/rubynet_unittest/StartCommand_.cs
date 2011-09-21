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
            logger = new Log4NetLogger(ConsoleLogger.ForCurrentProcess.Logger);
            command_line = CommandLine.ForCurrentProcess;
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowArgumentNullExceptionWhenTypeIsNull() {
            StartCommand command = new StartCommand(null, string.Empty, string.Empty);
        }

        [Test]
        public void ShouldCreateAnInstanceEvenWhenPipeNameIsNull() {
            StartCommand command = new StartCommand(typeof(RunAndStopService), null, string.Empty);
        }

        [Test]
        public void ShouldCreateAnInstanceEvenWhenCommandLineIsNull() {
            StartCommand command = new StartCommand(typeof(RunAndStopService), string.Empty, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldThrowArgumentExceptionWhenTypeDoesNotImplementsIRubyServiceInterface() {
            StartCommand command = new StartCommand(typeof(NonRubyService), string.Empty, null);
        }

        [Test]
        public void ShouldNotStartTheServiceIfDelayStartIsTrue() {
            StartCommand command = new StartCommand(typeof(RunAndStopService), null, null);
            IRubyServiceHost host = command.StartService(true, logger);
            Assert.AreEqual(ServiceStatus.Stopped, host.Service.Status);
        }

        [Test]
        public void ShouldStartTheServiceIfDelayStartIsFalse() {
            StartCommand command = new StartCommand(typeof(RunAndStopService), null, null);
            IRubyServiceHost host = command.StartService(false, logger);
            Assert.AreEqual(ServiceStatus.Running, host.Service.Status);
        }

        [Test]
        public void ShouldPropagateExceptionsThrowedByService() {
            StartCommand command = new StartCommand(typeof(ThrowExceptionWhenRunService), null, null);
            try {
                IRubyServiceHost host = command.StartService(false, logger);
                Assert.Fail();
            } catch {
                Assert.Pass();
            }
        }
    }
}
