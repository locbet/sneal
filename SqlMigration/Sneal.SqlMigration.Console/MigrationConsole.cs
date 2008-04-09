using Sneal.SqlMigration.Impl;

namespace Sneal.SqlMigration.Console
{
    public class MigrationConsole
    {
        private readonly ScriptMessageManager messageMgr = new ScriptMessageManager();

        public MigrationConsole()
        {
            messageMgr.ScriptMessage += MessageMgr_ScriptMessage;
        }

        public int Run(CmdLineScriptingOptions scriptingOptions, IConnectionSettings srcConnSettings,
                       IConnectionSettings targetConnSettings)
        {
            MigrationEngine engine = new MigrationEngine(new SqlServerScriptBuilder());
            engine.MessageManager = messageMgr;

            if (string.IsNullOrEmpty(targetConnSettings.Database))
            {
                engine.Script(srcConnSettings, scriptingOptions);
            }
            else
            {
                engine.ScriptDifferences(srcConnSettings, targetConnSettings, scriptingOptions);
            }

            return 0;
        }

        private void MessageMgr_ScriptMessage(object sender, ScriptMessageEventArgs e)
        {
            System.Console.WriteLine(e.Message);
        }
    }
}