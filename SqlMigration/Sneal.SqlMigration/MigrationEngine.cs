using MyMeta;
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
        private IDatabaseComparer dbComparer;
        private IScriptBuilder scriptBuilder;
        private SqlScript script;
        private IDatabase sourceDB;
        private IDatabase targetDB;

        public MigrationEngine(IScriptBuilder scriptBuilder, IDatabaseComparer dbComparer)
        {
            Should.NotBeNull(scriptBuilder, "scriptBuilder");
            Should.NotBeNull(dbComparer, "dbComparer");
            this.scriptBuilder = scriptBuilder;
            this.dbComparer = dbComparer;
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

            sourceDB = source;
            targetDB = target;

            messageManager.OnScriptMessage("Starting database differencing.");

            script = new SqlScript();

            ScriptNewTablesAndColumns();
            ScriptRemovedForeignKeys();
            ScriptRemovedTablesAndColumns();
            ScriptAlteredColumns();
            ScriptNewForeignKeys();
            ScriptNewIndexes();

            messageManager.OnScriptMessage("Finished database differencing.");

            return script;
        }

        protected virtual void ScriptNewTablesAndColumns()
        {
            messageManager.OnScriptMessage("Starting new table and column scripting...");

            foreach (ITable srcTable in sourceDB.Tables)
            {
                if (!dbComparer.Table(srcTable).ExistsIn(targetDB))
                {
                    messageManager.OnScriptMessage(
                        string.Format("Scripting missing table {0}", srcTable.Name));

                    script += scriptBuilder.Create(srcTable);
                }
                else
                {
                    ScriptNewColumns(srcTable);
                }
            }
        }

        protected virtual void ScriptNewColumns(ITable table)
        {
            foreach (IColumn column in table.Columns)
            {
                if (!dbComparer.Column(column).ExistsIn(targetDB))
                {
                    messageManager.OnScriptMessage(
                        string.Format("Scripting missing column {0} for table {1}", column.Name, table.Name));

                    script += scriptBuilder.Create(column);
                }
            }
        }

        protected virtual void ScriptNewForeignKeys()
        {
            messageManager.OnScriptMessage("Starting new foreign key scripting...");

            foreach (ITable srcTable in sourceDB.Tables)
            {
                foreach (IForeignKey fk in srcTable.ForeignKeys)
                {
                    if (!dbComparer.ForeignKey(fk).ExistsIn(targetDB))
                    {
                        messageManager.OnScriptMessage(
                            string.Format("Scripting missing foreign key {0} for table {1}", fk.Name, srcTable.Name));

                        script += scriptBuilder.Create(fk);
                    }
                }
            }
        }

        protected virtual void ScriptNewIndexes()
        {
            messageManager.OnScriptMessage("Starting new foreign key scripting...");

            foreach (ITable srcTable in sourceDB.Tables)
            {
                foreach (IIndex index in srcTable.Indexes)
                {
                    if (!dbComparer.Index(index).ExistsIn(targetDB))
                    {
                        messageManager.OnScriptMessage(
                            string.Format("Scripting missing index {0} for table {1}", index.Name, srcTable.Name));

                        script += scriptBuilder.Create(index);
                    }
                }
            }
        }

        protected virtual void ScriptRemovedForeignKeys()
        {
            messageManager.OnScriptMessage("Starting removed foreign key scripting.");

            foreach (ITable table in targetDB.Tables)
            {
                foreach (IForeignKey fk in table.ForeignKeys)
                {
                    if (!dbComparer.ForeignKey(fk).ExistsIn(sourceDB))
                    {
                        messageManager.OnScriptMessage(
                            string.Format("Scripting drop foreign key {0} for table {1}", fk.Name, table.Name));

                        script += scriptBuilder.Drop(fk);
                    }
                }
            }
        }

        protected virtual void ScriptRemovedColumns(ITable table)
        {
            foreach (IColumn column in table.Columns)
            {
                if (!dbComparer.Column(column).ExistsIn(sourceDB))
                {
                    messageManager.OnScriptMessage(
                        string.Format("Scripting drop column {0} for table {1}", column.Name, table.Name));

                    script += scriptBuilder.Drop(column);
                }
            }
        }

        protected virtual void ScriptRemovedTablesAndColumns()
        {
            messageManager.OnScriptMessage("Starting removed tables scripting...");

            foreach (ITable targetTable in targetDB.Tables)
            {
                if (!dbComparer.Table(targetTable).ExistsIn(sourceDB))
                {
                    messageManager.OnScriptMessage(
                        string.Format("Scripting drop table {0}", targetTable.Name));

                    script += scriptBuilder.Drop(targetTable);
                }
                else
                {
                    ScriptRemovedColumns(targetTable);
                }
            } 
        }

        protected virtual void ScriptRemovedIndexes()
        {
            messageManager.OnScriptMessage("Starting removed indexes scripting...");

            foreach (ITable targetTable in targetDB.Tables)
            {
                foreach (IIndex index in targetTable.Indexes)
                {
                    if (!dbComparer.Index(index).ExistsIn(sourceDB))
                    {
                        messageManager.OnScriptMessage(
                            string.Format("Scripting drop index {0} for table {1}", index.Name, targetTable.Name));

                        script += scriptBuilder.Drop(index);
                    }
                }
            }            
        }

        protected virtual void ScriptAlteredColumns()
        {
            messageManager.OnScriptMessage("Starting altered columns scripting...");

            // TODO: This requires temporarily dropping any FKs and indexes on the column before modification
        }
    }
}