using System.Collections.Generic;
using Sneal.Preconditions;

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
        private string database;
        private string password;
        private string serverName;
        private bool useIntegratedAuthentication = true;
        private string userName;

        public SqlServerConnectionSettings() {}

        public SqlServerConnectionSettings(string serverName, string database)
        {
            Throw.If(serverName, "serverName").IsEmpty();
            Throw.If(database, "database").IsEmpty();

            this.serverName = serverName;
            this.database = database;
        }

        public virtual string Password
        {
            get { return password; }
            set { password = value; }
        }

        public virtual string ServerName
        {
            get { return serverName; }
            set { serverName = value; }
        }

        public virtual bool UseIntegratedAuthentication
        {
            get { return useIntegratedAuthentication; }
            set { useIntegratedAuthentication = value; }
        }

        public virtual string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        #region IConnectionSettings Members

        public virtual string DriverType
        {
            get { return "SQL"; }
        }

        public virtual string ConnectionString
        {
            get
            {
                List<string> s = new List<string>();

                if (string.IsNullOrEmpty(ServerName))
                {
                    throw new SqlMigrationException(
                        "Cannot create connection string, ServerName is empty.");
                }
                if (string.IsNullOrEmpty(Database))
                {
                    throw new SqlMigrationException(
                        "Cannot create connection string, Database is empty.");
                }

                s.Add("Provider=SQLOLEDB.1");
                s.Add("Data Source=" + ServerName);
                s.Add("Initial Catalog=" + Database);

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
                    s.Add("Trusted_Connection=Yes");
                }

                return string.Join("; ", s.ToArray());
            }
        }

        public virtual string Database
        {
            get { return database; }
            set { database = value; }
        }

        #endregion
    }
}