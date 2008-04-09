using Sneal.CmdLineParser;
using Sneal.SqlMigration.Impl;

namespace Sneal.SqlMigration.Console
{
    /// <summary>
    /// Overridden so we can apply console switch attributes.
    /// </summary>
    public class TargetConnectionSettings : SqlServerConnectionSettings
    {
        [Switch("target-db", "The name of the target database.")]
        public override string Database
        {
            get { return base.Database; }
            set { base.Database = value; }
        }

        [Switch("target-server", "The name or IP address of the SQL Server.")]
        public override string ServerName
        {
            get { return base.ServerName; }
            set { base.ServerName = value; }
        }

        [Switch("target-integratedauth", "Use Windows integrated authentication.")]
        public override bool UseIntegratedAuthentication
        {
            get { return base.UseIntegratedAuthentication; }
            set { base.UseIntegratedAuthentication = value; }
        }

        [Switch("target-username", "The SQL Server username.")]
        public override string UserName
        {
            get { return base.UserName; }
            set { base.UserName = value; }
        }

        [Switch("target-password", "The SQL Server user password.")]
        public override string Password
        {
            get { return base.Password; }
            set { base.Password = value; }
        }
    }
}