using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

using Nohros.Data;

namespace Nohros.Ruby.Data
{
    public class SqlCommonDataProvider : CommonDataProvider
    {
        #region .ctor
        /// <summary>
        /// Initializes a new instance of the SqlCommonDataProvider by using the specified
        /// connection string and database owner.
        /// </summary>
        /// <param name="databaseOwner">The name of the database owner</param>
        /// <param name="connectionString">A string that can be used to connects to the database</param>
        public SqlCommonDataProvider(string databaseOwner, string connectionString):base(connectionString, databaseOwner)
        {
        }
        #endregion

        public SqlConnection GetSqlConnection()
        {
            return new SqlConnection(connectionString);
        }

        /// <summary>
        /// Retrieves the client password from the SQL server database.
        /// </summary>
        /// <param name="login">The login whose password is to be retrieve</param>
        /// <returns>The password of the specified login or null if the login is not
        /// found.</returns>
        public override string GetPwd(string login)
        {
            using(SqlConnection conn = GetSqlConnection())
            {
                SqlCommand cmd = new SqlCommand(databaseOwner + ".rb_session_getpwd", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@login", SqlDbType.VarChar, 80).Value = login;

                string pwd = (string)cmd.ExecuteScalar();

                conn.Close();

                return pwd;
            }
        }
    }
}