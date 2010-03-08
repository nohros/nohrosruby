using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.Threading;

namespace Nohros.Ruby
{
    public partial class RubyService : ServiceBase
    {
        public static SocketServer socketd;

        public RubyService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            System.Diagnostics.Debugger.Break();

            Thread sockets = new Thread(new ParameterizedThreadStart(delegate(object o)
            {
                ((SocketServer)o).Start();
            }));

            socketd = new SocketServer();
            sockets.Start(socketd);
        }

        protected override void OnStop()
        {
            // stoping the socket server
            socketd.Stop();

            // stopping the job server
        }
    }
}
