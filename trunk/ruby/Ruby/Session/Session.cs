using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;

using Nohros.Ruby.Data;

namespace Nohros.Ruby
{
    public class Session
    {
        Socket _fd;
        string _guid;
        bool _authenticated;
        SessionAuthInfo _auth;
        SocketServer _server;

        /* set to true to shuts the session down */
        internal bool ended;

        #region buffer elements
        internal byte[] inbuf; /* input buffer */
        internal int extra; /* last received data */
        internal int len; /* number of chars last readed */
        #endregion

        #region .ctor
        public Session()
        {
            _guid = Guid.NewGuid().ToString("N");
            inbuf = new byte[Const.RUBY_BUFFER_SIZE];
            extra = 0;
            len = 0;
            _auth = null;
        }
        #endregion

        #region Commands
        /// <summary>
        /// Authenticates the client
        /// </summary>
        /// <param name="id">The ID of the authentication command</param>
        /// <param name="sp">The name of the sevetiry package</param>
        /// <param name="sequence">Sequence value</param>
        /// <param name="data">The data associated with the step.</param>
        /// <returns></returns>
        internal bool USR(int id, string sp, char sequence, string data)
        {
            if (_authenticated)
            {
                SessionManager.Send(_fd, Resources.Session_Err_AlreadyLoggedIn);
                return true;
            }

            // validating the parameters
            if (id > 0 /* required */
                && data != null /* required */
                && ((sequence == 'I' && sp != null && sp == "MD5")/* required only when the sequence is equals to I */
                    || sequence == 'S') /* must be "I" or "S" */
                )
            {
                if (sequence == 'I')
                {
                    _auth = new SessionAuthInfo(data);

                    // phase 2
                    SessionManager.Send(_fd,
                        string.Concat(
                            "USR /Y",
                            id.ToString(),
                            "/S S /D",
                            _auth.Challenge)
                    );

                    CommonDataProvider dp = CommonDataProvider.Instance;
                    _auth.Password = dp.GetPwd(_auth.Login);
                }
                else if (sequence == 'S')
                {
                    // the authentication process was not initiated yet.
                    if (_auth == null) {
                        SessionManager.Send(_fd, Resources.Session_Err_SequenceInvalid);
                        return false;
                    }

                    string hash = Nohros.NSecurity.GetStringHash(_auth.Challenge + _auth.Password);
                    if (hash == data)
                    {
                        SessionManager.Send(_fd, string.Concat(
                            "USR /Y",
                            id,
                            "/D OK"));

                        _authenticated = true;
                    }
                }
                else
                {
                    SessionManager.Send(_fd, Resources.Session_Err_InvalidParameter);
                }
                return true;
            }

            SessionManager.Send(_fd, Resources.Session_Err_MissingParameter);
            return false;
        }
        #endregion

        public Socket Socket
        {
            get { return _fd; }
            internal set { _fd = value; }
        }

        /// <summary>
        /// Gets a <see cref="Guid"/> that uniquely identifies the Session.
        /// </summary>
        public string ID
        {
            get { return _guid; }
        }

        public SocketServer Server
        {
            get { return _server; }
            set { _server = value; }
        }

        /// <summary>
        /// Gets a value that determines whether a client is authenticated or not.
        /// </summary>
        public bool IsAuthenticated
        {
            get { return _authenticated; }
        }
    }
}