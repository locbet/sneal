using System.Collections.Generic;

namespace Sneal.SqlMigration.Impl
{
    /// <summary>
    /// Specifies RDMS login information. This class is SQL Server specific.
    /// </summary>
    /// <remarks>
    /// By default integrated authentication is used to login.
    /// </remarks>
    public class SqlServerConnectionSettings : IConnectionSettings
    {
        private readonly string database;
        private string password;
        private readonly string serverName;
        private bool useIntegratedAuthentication = true;
        private string userName;

        public SqlServerConnectionSettings(string serverName, string database)
        {
            this.serverName = serverName;
            this.database = database;
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public string ServerName
        {
            get { return serverName; }
        }

        public bool UseIntegratedAuthentication
        {
            get { return useIntegratedAuthentication; }
            set { useIntegratedAuthentication = value; }
        }

        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        #region IConnectionSettings Members

        public string DriverType
        {
            get { return "SQL"; }
        }

        public string ConnectionString
        {
            get
            {
                List<string> s = new List<string>();

                if (!string.IsNullOrEmpty(ServerName))
                    s.Add("Server=" + ServerName);

                if (!useIntegratedAuthentication)
                {
                    if (!string.IsNullOrEmpty(UserName))
                    {
                        s.Add("User ID=" + UserName);
                        s.Add("Password=" + Password);
                    }
                }
                else
                {
                    s.Add("Trusted_Connection=Yes;");
                }

                return string.Join("; ", s.ToArray());
            }
        }

        public string Database
        {
            get { return database; }
        }

        #endregion
    }
}