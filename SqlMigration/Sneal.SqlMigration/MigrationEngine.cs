using System;
using MyMeta;
using Sneal.Preconditions;
using Sneal.SqlMigration.Impl;

namespace Sneal.SqlMigration
{
    /// <summary>
    /// Main class for producing SQL scripts.
    /// </summary>
    public class MigrationEngine
    {
        private readonly IDatabaseComparer dbComparer;
        private readonly IScriptBuilder scriptBuilder;
        private IScriptMessageManager messageManager = new NullScriptMessageManager();
        private SqlScript script;
        private IDatabase sourceDB;
        private IDatabase targetDB;

        public MigrationEngine(IScriptBuilder scriptBuilder, IDatabaseComparer dbComparer)
        {
            Throw.If(scriptBuilder, "scriptBuilder").IsNull();
            Throw.If(dbComparer, "dbComparer").IsNull();

            this.scriptBuilder = scriptBuilder;
            this.dbComparer = dbComparer;
        }

        public IScriptMessageManager MessageManager
        {
            get { return messageManager; }
            set
            {
                Throw.If(value, "MessageManager").IsNull();
                messageManager = value;
            }
        }

        /// <summary>
        /// Returns an alter script to upgrade a database from target version
        /// to source version.
        /// </summary>
        /// <remarks>
        /// <para>This method is not thread safe.</para>
        /// <para>Operations are scripted in the following order:
        /// <list type="number">
        ///     <item>create new tables</item>
        ///     <item>add new columns to existing tables</item>
        ///     <item>drop removed foreign keys</item>
        ///     <item>drop removed columns</item>
        ///     <item>drop removed tables</item>
        ///     <item>alter any changed columns (like if you added a new default to a column)</item>
        ///     <item>add new foreign keys</item>
        ///     <item>add new indexes</item>
        ///     <item>functions</item>
        ///     <item>views</item>
        ///     <item>sprocs</item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <param name="source">The newest db.</param>
        /// <param name="target">The oldest db, the one to upgrade.</param>
        /// <returns>An alter SQL script.</returns>
        public virtual SqlScript ScriptDifferences(IDatabase source, IDatabase target)
        {
            Throw.If(source, "source").IsNull();
            Throw.If(target, "target").IsNull();

            sourceDB = source;
            targetDB = target;

            messageManager.OnScriptMessage("Starting database differencing.");

            script = new SqlScript();

            ScriptNewTablesAndColumns();
            ScriptRemovedForeignKeys();
            ScriptRemovedIndexes();
            ScriptRemovedTablesAndColumns();
            ScriptAlteredColumns();
            ScriptNewForeignKeys();
            ScriptNewIndexes();

            ScriptNewAndAlteredSprocs();
            ScriptRemovedSprocs();

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

        protected virtual void ScriptNewAndAlteredSprocs()
        {
            messageManager.OnScriptMessage("Starting stored procedure scripting...");

            foreach (IProcedure srcSproc in sourceDB.Procedures)
            {
                // TODO: filter by OLEDB PROCEDURE_TYPE to ensure we only script SPROCs?
//                if (dbComparer.Sproc(srcSproc).ExistsIn(targetDB))
//                {
//                    ScriptAlteredSproc(srcSproc);
//                }
//                else
//                {
//                    messageManager.OnScriptMessage(
//                        string.Format("Scripting create sproc {0}", srcSproc.Name));
//
//                    script += scriptBuilder.Create(srcSproc);                    
//                }
            }
        }

        protected virtual void ScriptAlteredSproc(IProcedure srcSproc)
        {
            IProcedure targetSproc = targetDB.Procedures[srcSproc.Name];

            if (srcSproc.ProcedureText != targetSproc.ProcedureText)
            {
                messageManager.OnScriptMessage(
                    string.Format("Scripting alter sproc {0}", srcSproc.Name));

                script += scriptBuilder.Alter(srcSproc);
            }
        }

        protected virtual void ScriptRemovedSprocs()
        {
            // TODO: implement
        }
    }
}