using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using log4net;

using Nohros.Logging;
using Nohros.Desktop;
using Nohros.Ruby.Service.Net;

namespace Nohros.Ruby.Tests.Net
{
    [TestFixture]
    public class StartCommand_
    {
        RubyNetShell shell = new RubyNetShell();
        ILog logger = ConsoleLogger.ForCurrentProcess.Logger;
        CommandLine command_line = CommandLine.ForCurrentProcess;

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowArgumentNullExceptionWhenCommandLineIsNull() {
            StartCommand command = new StartCommand(null, shell, logger);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowArgumentNullExceptionWhenRubyNetShellIsNull() {
            StartCommand command = new StartCommand(command_line, null, logger);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowArgumentNullExceptionWhenLoggerIsNull() {
            StartCommand command = new StartCommand(command_line, shell, null);
        }

        [Test]
        public void ShouldConstructNewStartCommand() {
            StartCommand command = new StartCommand(command_line, shell, logger);
        }
    }
}
