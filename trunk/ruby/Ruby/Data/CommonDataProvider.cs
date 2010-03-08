using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

using Nohros.Data;

namespace Nohros.Ruby.Data
{
    public abstract class CommonDataProvider : IDataProvider
    {
        protected string connectionString;
        protected string databaseOwner;

        static CommonDataProvider instance;

        /// <summary>
        /// Gets an instance of the CommonDataProvider class, which can be used to retrieve or persist data against a database.
        /// </summary>
        public static CommonDataProvider Instance
        {
            get
            {
                if (instance == null)
                {
                    Provider provider = (Provider)Configuration.Instance.Providers["CommonDataProvider"];

                    if (provider == null)
                        throw new ProviderException(Resources.InvalidConfigurationFile);

                    instance = DataProvider.CreateInstance<CommonDataProvider>(provider);
                }
                return Instance;
            }
        }

        #region .ctor
        /// <summary>
        /// Initialize a new instance of the CommonDataProvider by using the specified
        /// connection string and database owner
        /// </summary>
        /// <param name="connectionString">A string that can be used to connect to the database</param>
        /// <param name="databaseOwner">A string that represents the name od the database owner</param>
        protected CommonDataProvider(string connectionString, string databaseOwner)
        {
            this.connectionString = connectionString;
            this.databaseOwner = databaseOwner;
        }
        #endregion

        #region IDataProvider

        /// <summary>
        /// Gets a string that can be used to connect to the database.
        /// </summary>
        public string ConnectionString
        {
            get { return this.connectionString; }
        }

        /// <summary>
        /// Gets the name of the database owner.
        /// </summary>
        public string DatabaseOwner
        {
            get { return this.databaseOwner; }
        }
        #endregion

        #region Session
        /// <summary>
        /// Retrieve the hash of password related with the specified login.
        /// </summary>
        /// <param name="login">The login whose password to be retrieved</param>
        /// <returns>The hash of the password related with the <paramref name="login"/></returns>
        public abstract string GetPwd(string login);
        #endregion
    }
}