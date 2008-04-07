using Sneal.CmdLineParser;

namespace Sneal.SqlMigration.Console
{
    public class Options
    {
        private bool scriptData;
        private bool scriptSprocs;
        private bool scriptTableSchema;
        private bool scriptViews;
        private string server;
        private string password;
        private string userName;
        private string targetServer;
        private string targetPassword;
        private string targetUserName;
        private bool showHelp;

        [Switch("server", "The name or IP address of the source DB server.")]
        public string Server
        {
            get { return server; }
            set { server = value; }
        }

        [Switch("username", "The user name to login to the source DB server.")]
        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        [Switch("password", "The user password to login to the source DB server.")]
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        [Switch("targetserver", "The name or IP address of the source DB server.")]
        public string TargetServer
        {
            get { return targetServer; }
            set { targetServer = value; }
        }

        [Switch("targetusername", "The user name to login to the source DB server.")]
        public string TargetUserName
        {
            get { return targetUserName; }
            set { targetUserName = value; }
        }

        [Switch("targetpassword", "The user password to login to the source DB server.")]
        public string TargetPassword
        {
            get { return targetPassword; }
            set { targetPassword = value; }
        }

        [Switch("help", "Shows the help command line usage.")]
        public bool ShowHelp
        {
            get { return showHelp; }
            set { showHelp = value; }
        }

        [Switch("scriptdata", "Flag to script table data.")]
        public bool ScriptData
        {
            get { return scriptData; }
            set { scriptData = value; }
        }

        [Switch("scriptdata", "Flag to script table DDL.")]
        public bool ScriptTableSchema
        {
            get { return scriptTableSchema; }
            set { scriptTableSchema = value; }
        }

        [Switch("scriptdata", "Flag to script stored procedures.")]
        public bool ScriptSprocs
        {
            get { return scriptSprocs; }
            set { scriptSprocs = value; }
        }

        [Switch("scriptdata", "Flag to script views.")]
        public bool ScriptViews
        {
            get { return scriptViews; }
            set { scriptViews = value; }
        }
    }
}