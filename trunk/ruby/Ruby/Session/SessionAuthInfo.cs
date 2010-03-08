using System;
using System.Collections.Generic;
using System.Text;

namespace Nohros.Ruby
{
    internal class SessionAuthInfo
    {
        public string _login;
        public string _pwd;
        public string _challenge;

        #region .ctor
        /// <summary>
        /// Initiates a new instance of the SessionAuthInfo class by using the specified
        /// client login.
        /// </summary>
        /// <param name="login">The login used to identify the session client</param>
        public SessionAuthInfo(string login)
        {
            _login = login;
            _challenge = Guid.NewGuid().ToString("N");
        }

        /// <summary>
        /// Initiates a new instance of the SessionAuthInfo class by using the specified
        /// client login and password hash.
        /// </summary>
        /// <param name="login">The login used to identify the client</param>
        /// <param name="pwdhash">The password of the client related with the
        /// <paramref name="login"/></param>
        public SessionAuthInfo(string login, string pwd):this(login)
        {
            _pwd = pwd;
        }
        #endregion

        /// <summary>
        /// Gets a string that identifies the session client.
        /// </summary>
        public string Login
        {
            get { return _login; }
        }

        /// <summary>
        /// Gets client password.
        /// </summary>
        public string Password
        {
            get { return _pwd; }
            set { _pwd = value; }
        }

        /// <summary>
        /// Gets a string that can be used like a challenge string in the
        /// authentication process.
        /// </summary>
        public string Challenge
        {
            get { return _challenge; }
        }
    }
}