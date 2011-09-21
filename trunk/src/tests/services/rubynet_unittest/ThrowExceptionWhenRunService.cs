using System;
using System.Collections.Generic;
using System.Text;

namespace Nohros.Ruby.Tests.Net
{
    public class ThrowExceptionWhenRunService : IRubyService
    {
        ServiceStatus service_status_;
        ILog logger_;
        string command_line_;

        #region .ctor()
        public ThrowExceptionWhenRunService(string command_line, ILog logger) {
            command_line_ = command_line;
            service_status_ = 0;
            logger_ = logger;
        }
        #endregion

        public void Run() {
            throw new Exception();
        }

        public void Stop() {
            service_status_ = ServiceStatus.Running;
        }

        public bool OnServerMessage(IRubyMessagePacket message) {
            return true;
        }

        public string Name {
            get { return "ThrowExceptionWhenRunService"; }
        }

        public ServiceStatus Status {
            get { return service_status_; }
        }
    }
}