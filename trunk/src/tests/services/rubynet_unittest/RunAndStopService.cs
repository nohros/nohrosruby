using System;
using System.Collections.Generic;
using System.Text;

using Nohros.Ruby.Service;

namespace Nohros.Ruby.Tests.Net
{
    public class RunAndStopService : IRubyService
    {
        ServiceStatus service_status_;
        string command_line_;
        ILog logger_;

        #region .ctor()
        public RunAndStopService(string command_line, ILog logger) {
            command_line_ = command_line;
            service_status_ = ServiceStatus.Stopped;
            logger_ = logger;
        }
        #endregion

        public void Run() {
            service_status_ = ServiceStatus.Running;
        }

        public void Stop() {
            service_status_ = ServiceStatus.Stopped;
        }

        public bool OnServerMessage(IRubyMessagePacket message) {
            return true;
        }

        public string Name {
            get { return "RunAndStopService"; }
        }

        public ServiceStatus Status {
            get { return service_status_; }
        }
    }
}
