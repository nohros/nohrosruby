using System;
using System.Collections.Generic;
using System.Text;

using Nohros.Ruby.Service;

namespace Nohros.Ruby.Tests.Net
{
    public class WaitService : IRubyService
    {
        ServiceStatus service_status_;
        string command_line_;

        #region .ctor()
        public WaitService(string command_line) {
            command_line_ = command_line;
            service_status_ = 0;
        }
        #endregion

        public void Run() {
            service_status_ = ServiceStatus.Running;
            System.Threading.Thread.Sleep(100);
        }

        public void Stop() {
            service_status_ = ServiceStatus.Running;
        }

        public bool OnServerMessage(IRubyMessagePacket message) {
            return true;
        }

        public string Name {
            get { return "WaitService"; }
        }

        public ServiceStatus Status {
            get { return service_status_; }
        }
    }
}
