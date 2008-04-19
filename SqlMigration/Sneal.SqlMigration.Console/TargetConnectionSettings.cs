using Sneal.CmdLineParser;
using Sneal.SqlMigration.Impl;

namespace Sneal.SqlMigration.Console
{
    /// <summary>
    /// Overridden so we can apply console switch attributes.
    /// </summary>
    public class TargetConnectionSettings : SqlServerConnectionSettings
    {
        [Switch("targetdb", "The name of the target database.")]
        public override string Database
        {
            get { return base.Database; }
            set { base.Database = value; }
        }

        [Switch("targetserver", "The name or IP address of the SQL Server.")]
        public override string ServerName
        {
            get { return base.ServerName; }
            set { base.ServerName = value; }
        }

        [Switch("targetusername", "The SQL Server username.")]
        public override string UserName
        {
            get { return base.UserName; }
            set { base.UserName = value; }
        }

        [Switch("targetpassword", "The SQL Server user password.")]
        public override string Password
        {
            get { return base.Password; }
            set { base.Password = value; }
        }
    }
}