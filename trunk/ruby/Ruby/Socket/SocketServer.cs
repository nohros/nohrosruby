using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using log4net;

namespace Nohros.Ruby
{
    /// <summary>
    /// Implements server sockets. A server socket waits for requests to come in over the network. It performs
    /// some operation based on that request, and then possibly returns a result to the requester.
    /// </summary>
    public sealed class SocketServer
    {
        Hashtable sessions;
        IList<Socket> sockets;
        Socket listener;
        bool connected;

        static ManualResetEvent sync;

        #region .ctor
        public SocketServer()
        {
            Configuration config = Configuration.Instance;
            IPAddress[] address = Dns.GetHostAddresses(config.IP);

            sessions = new Hashtable();
            sockets = new List<Socket>(30);
            sync = new ManualResetEvent(false);
            connected = false;
        }
        #endregion

        /// <summary>
        /// Starts listening for incoming connection requests.
        /// </summary>
        public void Start()
        {
            Configuration config = Configuration.Instance;

            // create our "listening" socket
            IPAddress[] address = Dns.GetHostAddresses(config.IP);
            IPEndPoint endpoint = new IPEndPoint(address[0], config.Port);
            listener = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            
            // lose the pesky "address already in use" error message.
            listener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            listener.Bind(endpoint);
            listener.Listen(1000);

            connected = true;

            // main loop
            while (connected)
            {
                sync.Reset();

                // listening for connectionis asynchronous mode
                listener.BeginAccept(new AsyncCallback(Accept), listener);
                
                // wait for a client connection
                sync.WaitOne();
            }
        }

        /// <summary>
        /// Stop the server nad release all associated resources.
        /// </summary>
        public void Stop()
        {
            connected = false;

            /* stop listening */
            listener.Close(3000);

            /* close active sessions */
            IDictionaryEnumerator enumerator = sessions.GetEnumerator();
            while (enumerator.MoveNext()) {
                SessionManager.Close((Session)enumerator.Value);
            }

            /* ensure that all threads terminate */
            sync.WaitOne(3000);
        }

        /// <summary>
        /// Accept an incoming connecion attempt and starts the data receiver operation.
        /// </summary>
        /// <param name="state">Object containing state information about the request</param>
        public void Accept(IAsyncResult state)
        {
            /* accepts an incoming connection attempt ... */
            Socket newfd, fd = (Socket)state.AsyncState;

            /* and creates a Socket to handle the communication. */
            try { newfd = fd.EndAccept(state); }
            catch (ObjectDisposedException) {
                LogManager.GetLogger(Const.RUBY_LOGGER_NAME).Info(Resources.Socket_Closed);
                newfd = null;
            }
            catch (Exception ex ) {
                LogManager.GetLogger(Const.RUBY_LOGGER_NAME).Error(ex.Message, ex);
                newfd = null;
            }

            /* creates a new Session to handle the data transfer operation */
            if (newfd != null)
            {
                Session session = SessionManager.NewSession(newfd, this);
                sessions.Add(session.ID, session);
            }

            /* allow more clients to connect */
            sync.Set();
        }

        /// <summary>
        /// Remove a session from the sessions table.
        /// </summary>
        public void RemoveSession(string id)
        {
            sessions.Remove(id);
        }
    }
}