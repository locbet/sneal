using Sneal.SqlMigration.Utils;

namespace Sneal.SqlMigration.Impl
{
    public class DefaultDatabaseConnectionInfo : IDatabaseConnectionInfo
    {
        private string instance;
        private string name;
        private string password;
        private string port;
        private string server = "localhost";
        private string userName;
        private bool useTrusedConnection;

        #region IDatabaseConnectionInfo Members

        public string Name
        {
            get { return name; }
            set
            {
                Should.NotBeNullOrEmpty(value, "Name");
                name = value;
            }
        }

        public string Instance
        {
            get { return instance; }
            set { instance = value; }
        }

        public string Server
        {
            get { return server; }
            set
            {
                Should.NotBeNullOrEmpty(value, "Server");
                server = value;
            }
        }

        public string Port
        {
            get { return port; }
            set { port = value; }
        }

        public bool UseTrusedConnection
        {
            get { return useTrusedConnection; }
            set { useTrusedConnection = value; }
        }

        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        #endregion
    }
}