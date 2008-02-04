using Sneal.SqlMigration.Impl;
using Sneal.SqlMigration.Utils;

namespace Sneal.SqlMigration
{
    /// <summary>
    /// Main class for producing SQL scripts.
    /// </summary>
    public class MigrationEngine
    {
        private IScriptMessageManager messageManager = new NullScriptMessageManager();
        private SqlScript script;
        private IScriptBuilder scriptBuilder;

        public MigrationEngine(IScriptBuilder scriptBuilder)
        {
            Should.NotBeNull(scriptBuilder, "scriptBuilder");
            this.scriptBuilder = scriptBuilder;
        }

        public IScriptMessageManager MessageManager
        {
            get { return messageManager; }
            set
            {
                Should.NotBeNull(value, "MessageManager");
                messageManager = value;
            }
        }

        /// <summary>
        /// Returns an alter script to upgrade a database from target version
        /// to source version.
        /// </summary>
        /// <remarks>
        /// 1.  create new tables
        /// 2.  add new columns to existing tables
        /// 3.  drop removed foreign keys
        /// 4.  drop removed columns
        /// 5.  drop removed tables
        /// 6.  alter any changed columns (like if you added a new default to a column)
        /// 7.  add new foreign keys
        /// 8.  add new indexes
        /// 9.  functions
        /// 10. views
        /// 11. sprocs
        /// </remarks>
        /// <param name="source">The newest db.</param>
        /// <param name="target">The oldest db, the one to upgrade.</param>
        /// <returns>An alter SQL script.</returns>
        public virtual SqlScript ScriptDifferences(IDatabase source, IDatabase target)
        {
            Should.NotBeNull(source, "source");
            Should.NotBeNull(target, "target");

            // just to make sure these are set correctly
            source.IsLatestVersion = true;
            target.IsLatestVersion = false;

            messageManager.OnScriptMessage("Starting database differencing.");

            script = new SqlScript();

            ScriptNewTables();
            ScriptNewColumns();
            ScriptRemovedForeignKeys();
            ScriptRemovedColumns();
            ScriptRemovedTables();
            ScriptAlteredColumns();
            ScriptNewForeignKeys();
            ScriptNewIndexes();

            messageManager.OnScriptMessage("Finished database differencing.");

            return script;
        }

        protected virtual void ScriptNewTables()
        {
            messageManager.OnScriptMessage("Starting new table scripting.");
        }

        protected virtual void ScriptNewColumns()
        {
            messageManager.OnScriptMessage("Starting new column scripting.");
        }

        protected virtual void ScriptRemovedForeignKeys()
        {
            messageManager.OnScriptMessage("Starting removed foreign key scripting.");
        }

        protected virtual void ScriptRemovedColumns()
        {
            messageManager.OnScriptMessage("Starting removed columns scripting.");
        }

        protected virtual void ScriptRemovedTables()
        {
            messageManager.OnScriptMessage("Starting removed tables scripting.");
        }

        protected virtual void ScriptAlteredColumns()
        {
            messageManager.OnScriptMessage("Starting altered columns scripting.");
        }

        protected virtual void ScriptNewForeignKeys()
        {
            messageManager.OnScriptMessage("Starting new foreign key scripting.");
        }

        protected virtual void ScriptNewIndexes()
        {
            messageManager.OnScriptMessage("Starting new indexes scripting.");
        }
    }
}