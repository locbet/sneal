using Sneal.CmdLineParser;
using Sneal.SqlMigration.Impl;

namespace Sneal.SqlMigration.Console
{
    /// <summary>
    /// Overridden so we can apply console switch attributes.
    /// </summary>
    public class SourceConnectionSettings : SqlServerConnectionSettings
    {
        [Switch("db", "The name of the source database.")]
        public override string Database
        {
            get { return base.Database; }
            set { base.Database = value; }
        }

        [Switch("server", "The name or IP address of the SQL Server.")]
        public override string ServerName
        {
            get { return base.ServerName; }
            set { base.ServerName = value; }
        }

        [Switch("integratedauth", "Use Windows integrated authentication.")]
        public override bool UseIntegratedAuthentication
        {
            get { return base.UseIntegratedAuthentication; }
            set { base.UseIntegratedAuthentication = value; }
        }

        [Switch("username", "The SQL Server username.")]
        public override string UserName
        {
            get { return base.UserName; }
            set { base.UserName = value; }
        }

        [Switch("password", "The SQL Server user password.")]
        public override string Password
        {
            get { return base.Password; }
            set { base.Password = value; }
        }
    }
}