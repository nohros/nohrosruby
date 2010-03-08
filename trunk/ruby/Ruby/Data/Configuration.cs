using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.IO;
using System.Xml;

using Nohros.Data;
using Nohros.Configuration;

namespace Nohros.Ruby
{
    public class Configuration : IConfiguration
    {
        static Configuration instance;
        static object _lock;
        static string _configFile;

        long _idleTimeout;
        int _port;
        string _ip;
        Hashtable _providers;

        #region .ctor
        /// <summary>
        /// Static constructor
        /// </summary>
        static Configuration()
        {
            string path = ConfigurationManager.AppSettings["ConfigurationFile"];
            if (path.StartsWith("~"))
                path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path.Substring(2));

            if (!File.Exists(path))
                throw new ConfigurationErrorsException(StringResources.GetString(string.Format(StringResources.Config_FileNotFound_Path, path)));
            
            _configFile = path;
            _lock = new object();
        }

        /// <summary>
        /// Initializes a new instance of the Configuration class
        /// </summary>
        public Configuration()
        {
            _providers = new Hashtable();
            _idleTimeout = 0;
            _port = 8955;
            _ip = "127.0.0.1";
        }
        #endregion

        /// <summary>
        /// Load ans parse the configuration file.
        /// </summary>
        public override void Load()
        {
            // load the Xml configuration file on memory
            XmlDocument doc = new XmlDocument();
            doc.Load(_configFile);

            Load((XmlElement)doc.DocumentElement);

            // monitor the configuration file for changes
            FileInfo file = new FileInfo(_configFile);
            Watch(file);

            // get the data providers
            GetProviders(doc.SelectSingleNode("//providers"));
        }

        /// <summary>
        /// Gets an instance of the Configuration class, which can be used to retrieve the
        /// the application configuration values.
        /// </summary>
        public static Configuration Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (_lock)
                    {
                        if (instance == null)
                        {
                            instance = new Configuration();
                            instance.Load();
                        }
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// Parse the data providers
        /// </summary>
        /// <param name="node">A XML node containing the data providers to parse</param>
        /// <exception cref="ArgumentNullException">node is null</exception>
        protected void GetProviders(XmlNode node)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            foreach(XmlNode provider in node.ChildNodes)
            {
                _providers.Add(provider.Attributes["name"].Value, new Provider(provider.Attributes));
            }
        }

        /// <summary>
        /// Gets a list of available data providers.
        /// </summary>
        internal Hashtable Providers
        {
            get { return _providers; }
        }

        /// <summary>
        /// Gets the maximum idle timeout
        /// </summary>
        public long IdleTimeout
        {
            get { return _idleTimeout; }
            internal set { _idleTimeout = value; }
        }

        /// <summary>
        /// Gets the port number used by the socket server.
        /// </summary>
        public int Port
        {
            get { return _port; }
            internal set { _port = value; }
        }

        /// <summary>
        /// The IP address used by the of the socket server.
        /// </summary>
        public string IP
        {
            get { return _ip; }
            internal set { _ip = value; }
        }
    }
}