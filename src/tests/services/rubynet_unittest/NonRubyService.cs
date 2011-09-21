using System;
using System.Collections.Generic;
using System.Text;

using Nohros.Ruby.Service;

namespace Nohros.Ruby.Tests.Net
{
    public class NonRubyService
    {
        ServiceStatus service_status_;
        string command_line_;

        #region .ctor()
        public NonRubyService(string command_line) {
            command_line_ = command_line;
            service_status_ = 0;
        }
        #endregion

        public void Run() {
            service_status_ = ServiceStatus.Running;
        }

        public void Stop() {
            service_status_ = ServiceStatus.Running;
        }

        public bool OnServerMessage(IRubyMessagePacket message) {
            return true;
        }

        public string Name {
            get { return "NonRubyService"; }
        }

        public ServiceStatus Status {
            get { return service_status_; }
        }
    }
}
