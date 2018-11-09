using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Extensions;

namespace UHub.CoreLib.DataInterop
{
    /// <summary>
    /// Configuration tool to simplify the construction of connection strings
    /// </summary>
    public sealed class SqlConfig
    {
        private bool? _isValid = null;
        private string _connectionString = null;

        /// <summary>
        /// Server name
        /// </summary>
        private string Server { get; }

        /// <summary>
        /// Database name
        /// </summary>
        private string Database { get; }

        /// <summary>
        /// Flag to determine whether the connection should allow async processing
        /// </summary>
        private bool EnableAsync { get; }

        /// <summary>
        /// Flag to determine if integrated security should be used or SQL authentication
        /// </summary>
        private bool UseIntegratedSecurity { get; }

        /// <summary>
        /// Connection username for SQL auth
        /// </summary>
        private string Username { get; }

        /// <summary>
        /// Connection password for SQL auth
        /// </summary>
        private string Password { get; }


        /// <summary>
        /// Initializer
        /// </summary>
        /// <param name="Server">SQL server name or IP</param>
        /// <param name="Database">SQL DB name</param>
        /// <param name="UseIntegratedSecurity"></param>
        public SqlConfig(string Server, string Database, bool EnableAsync, bool UseIntegratedSecurity) :
            this(Server, Database, EnableAsync, UseIntegratedSecurity, "", "")
        {

        }
        /// <summary>
        /// Initializer
        /// </summary>
        /// <param name="Server">SQL server name or IP</param>
        /// <param name="Database">SQL DB name</param>
        /// <param name="Username">SQL username</param>
        /// <param name="Password">SQL user password</param>
        public SqlConfig(string Server, string Database, bool EnableAsync, string Username, string Password) :
            this(Server, Database, EnableAsync, false, Username, Password)
        {

        }
        /// <summary>
        /// Initializer
        /// </summary>
        /// <param name="Server">SQL server name or IP</param>
        /// <param name="Database">SQL DB name</param>
        /// <param name="UseIntegratedSecurity"></param>
        /// <param name="Username">SQL username</param>
        /// <param name="Password">SQL user password</param>
        public SqlConfig(string Server, string Database, bool EnableAsync, bool UseIntegratedSecurity, string Username, string Password)
        {
            this.Server = Server;
            this.Database = Database;
            this.EnableAsync = EnableAsync;
            this.UseIntegratedSecurity = UseIntegratedSecurity;
            this.Username = Username;
            this.Password = Password;
        }
        /// <summary>
        /// Initialiazer
        /// </summary>
        /// <param name="ConnectionString"></param>
        public SqlConfig(string ConnectionString)
        {
            this._connectionString = ConnectionString;
        }

        /// <summary>
        /// Create copy of existing SQL config
        /// </summary>
        /// <param name="config"></param>
        internal SqlConfig(SqlConfig config)
        {
            if (config._connectionString.IsEmpty())
            {
                this.Server = config.Server;
                this.Database = config.Database;
                this.EnableAsync = config.EnableAsync;
                this.UseIntegratedSecurity = config.UseIntegratedSecurity;
                this.Username = config.Username;
                this.Password = config.Password;
            }
            else
            {
                this._connectionString = config._connectionString;
            }
            this._isValid = config._isValid;
        }

        /// <summary>
        /// Get full connection string for DB connection
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (_connectionString != null)
            {
                return _connectionString;
            }

            return GetConnectionString();
        }

        /// <summary>
        /// Auto cast SqlConfig to string for DB connection
        /// </summary>
        /// <seealso cref="SqlConfig.ToString()"/>
        /// <param name="config"></param>
        public static implicit operator string(SqlConfig config)
        {
            return config.ToString();
        }

        /// <summary>
        /// Ensure that all properties are valid
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        private bool IsValid()
        {
            if (_isValid != null)
            {
                return _isValid.Value;
            }

            if (Server.IsEmpty())
            {
                _isValid = false;
                throw new ArgumentException("Server cannot be null or empty");
            }
            if (Database.IsEmpty())
            {
                _isValid = false;
                throw new ArgumentException("Database cannot be null or empty");
            }

            if (!UseIntegratedSecurity)
            {
                if (Username.IsEmpty())
                {
                    _isValid = false;
                    throw new ArgumentException("Username cannot be null or empty");
                }
                if (Password.IsEmpty())
                {
                    _isValid = false;
                    throw new ArgumentException("Password cannot be null or empty");
                }

            }

            if (Username.IsEmpty() && Password.IsNotEmpty())
            {
                _isValid = false;
                throw new ArgumentException("Username cannot be null or empty");
            }

            _isValid = true;
            return true;
        }


        /// <summary>
        /// Get connection string from properties
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        public string GetConnectionString()
        {
            if (_connectionString != null)
            {
                return _connectionString;
            }

            if (!IsValid())
            {
                throw new InvalidOperationException("Cannot get connection string from invalid config");
            }

            StringBuilder builder = new StringBuilder();

            //server
            builder.Append("Server=");
            builder.Append(Server);
            builder.Append(";");
            //database
            builder.Append("Database=");
            builder.Append(Database);
            builder.Append(";");
            //Async
            if(EnableAsync)
            {
                builder.Append("Asynchronous Processing=True;");
            }

            //IntegratedSecurity
            if (UseIntegratedSecurity)
            {
                builder.Append("Integrated Security=SSPI;");
            }
            //username
            if (Username.IsNotEmpty())
            {
                builder.Append("Uid=");
                builder.Append(Username);
                builder.Append(";");
            }
            //password
            if (Password.IsNotEmpty())
            {
                string psdAdj;
                if (Password.Contains(";"))
                {
                    if (Password.StartsWith("'"))
                    {
                        psdAdj = "\"" + Password + "\"";
                    }
                    else
                    {
                        psdAdj = "'" + Password + "'";
                    }
                }
                else
                {
                    psdAdj = Password;
                }

                builder.Append("Pwd=");
                builder.Append(psdAdj);
                builder.Append(";");
            }


            _connectionString = builder.ToString();
            return _connectionString;
        }

        /// <summary>
        /// Test connection to ensure that the DB can be accessed
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        public bool ValidateConnection()
        {
            string connection = GetConnectionString();

            try
            {
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open(); // throws if invalid
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
