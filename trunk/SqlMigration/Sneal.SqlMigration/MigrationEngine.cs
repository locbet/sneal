using System;
using MyMeta;
using Sneal.Preconditions;
using Sneal.SqlMigration.Impl;
using Sneal.SqlMigration.Migrators;

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

        /// <summary>
        /// Creates a new migration engine instance.
        /// </summary>
        /// <param name="scriptBuilder">The script builder instance.</param>
        /// <param name="dbComparer">The DB comparer instance.</param>
        public MigrationEngine(IScriptBuilder scriptBuilder, IDatabaseComparer dbComparer)
        {
            Throw.If(scriptBuilder, "scriptBuilder").IsNull();
            Throw.If(dbComparer, "dbComparer").IsNull();

            this.scriptBuilder = scriptBuilder;
            this.dbComparer = dbComparer;
        }

        /// <summary>
        /// The message manager instance that is used to write scripting
        /// messages to.
        /// </summary>
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
        /// This event is fired every time an object is scripted, returning
        /// the total object count and the count already scripted.
        /// </summary>
        public event EventHandler<ProgressEventArgs> ProgressEvent;

        /// <summary>
        /// Scripts a database to disk for use in creating a brand new db.  This
        /// method scripts the entire object, i.e. no differntial or comparisons
        /// are done.
        /// </summary>
        /// <param name="connectionInfo">The db connection parameters.</param>
        /// <param name="scriptingOptions">The export scripting settings.</param>
        public virtual void Script(IConnectionSettings connectionInfo, IScriptingOptions scriptingOptions)
        {
            Throw.If(connectionInfo, "connectionInfo").IsNull();
            Throw.If(scriptingOptions, "scriptingOptions").IsNull();

            IDatabase db = DatabaseConnectionFactory.CreateDbConnection(connectionInfo);

            IScriptWriter writer;
            if (scriptingOptions.UseMultipleFiles)
                writer = new MultiFileScriptWriter(scriptingOptions.ExportDirectory);
            else
                writer = new SingleFileScriptWriter(scriptingOptions.ExportDirectory);

            int totalObjects = CalculateScriptObjectCount(scriptingOptions);
            int exportCount = 0;

            foreach (string tableName in scriptingOptions.TablesToScript)
            {
                ITable table = db.Tables[tableName];
                if (table == null)
                {
                    throw new SqlMigrationException(
                        string.Format("Unable to find the table {0} in database {1} on server.",
                                      tableName, connectionInfo.Database));
                }

                // TODO: constraints?

                if (scriptingOptions.ScriptSchema)
                {
                    ScriptTableSchema(table, writer);
                    OnProgressEvent(++exportCount, totalObjects);
                }

                if (scriptingOptions.ScriptIndexes)
                {
                    ScriptTableIndexes(table, writer);
                    OnProgressEvent(++exportCount, totalObjects);
                }

                if (scriptingOptions.ScriptData)
                {
                    ScriptTableData(table, writer);
                    OnProgressEvent(++exportCount, totalObjects);
                }
            }

            foreach (string sprocName in scriptingOptions.SprocsToScript)
            {
                IProcedure sproc = db.Procedures[sprocName];
                if (sproc == null)
                {
                    throw new SqlMigrationException(
                        string.Format("Unable to find the procedure {0} in database {1} on server.",
                                      sprocName, connectionInfo.Database));
                }
                ScriptSproc(sproc, writer);
                OnProgressEvent(++exportCount, totalObjects);
            }

            foreach (string viewName in scriptingOptions.ViewsToScript)
            {
                IView view = db.Views[viewName];
                if (view == null)
                {
                    throw new SqlMigrationException(
                        string.Format("Unable to find the view {0} in database {1} on server.",
                                      viewName, connectionInfo.Database));
                }
                ScriptView(view, writer);
                OnProgressEvent(++exportCount, totalObjects);
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
        /// <param name="connectionInfoSourceDb">The newer, source db.</param>
        /// <param name="connectionInfoTargetDb">The older, target db to upgrade.</param>
        /// <param name="scriptingOptions">The export scripting settings.</param>
        public virtual void ScriptDifferences(IConnectionSettings connectionInfoSourceDb,
                                              IConnectionSettings connectionInfoTargetDb,
                                              IScriptingOptions scriptingOptions)
        {
            throw new NotImplementedException("Script differencing is not yet supported.");

//            Throw.If(source, "source").IsNull();
//            Throw.If(target, "target").IsNull();
//
//            sourceDB = source;
//            targetDB = target;
//
//            messageManager.OnScriptMessage("Starting database differencing.");
//
//            script = new SqlScript();
//
//            ScriptNewTablesAndColumns();
//            ScriptRemovedForeignKeys();
//            ScriptRemovedIndexes();
//            ScriptRemovedTablesAndColumns();
//            ScriptAlteredColumns();
//            ScriptNewForeignKeys();
//            ScriptNewIndexes();
//
//            ScriptNewAndAlteredSprocs();
//            ScriptRemovedSprocs();
//
//            messageManager.OnScriptMessage("Finished database differencing.");
        }

        protected virtual void OnProgressEvent(ProgressEventArgs progressEventArgs)
        {
            Throw.If(progressEventArgs, "progressEventArgs").IsNull();

            EventHandler<ProgressEventArgs> evt = ProgressEvent;
            if (evt != null)
            {
                evt(this, progressEventArgs);
            }
        }

        protected virtual void OnProgressEvent(int objectsCompleted, int totalObject)
        {
            EventHandler<ProgressEventArgs> evt = ProgressEvent;
            if (evt != null)
            {
                evt(this, new ProgressEventArgs(objectsCompleted, totalObject));
            }
        }

        protected static int CalculateScriptObjectCount(IScriptingOptions options)
        {
            Throw.If(options, "options").IsNull();

            int tableWeight = options.ScriptSchema ? 1 : 0;
            tableWeight += options.ScriptIndexes ? 1 : 0;
            //tableWeight += options.ScriptConstraints ? 1 : 0;
            tableWeight += options.ScriptData ? 1 : 0;

            int exportObjectTotal = options.TablesToScript.Count*tableWeight;
            exportObjectTotal += options.ViewsToScript.Count;
            exportObjectTotal += options.SprocsToScript.Count;

            return exportObjectTotal;
        }

        #region Script All Objects

        protected void ScriptTableSchema(ITable table, IScriptWriter writer)
        {
            Throw.If(table, "table").IsNull();
            Throw.If(writer, "writer").IsNull();

            string msg = string.Format("Scripting {0} table schema", table.Name);
            messageManager.OnScriptMessage(msg);

            SqlScript script = scriptBuilder.Create(table);
            writer.WriteTableScript(table.Name, script.ToScript());
        }

        protected void ScriptTableIndexes(ITable table, IScriptWriter writer)
        {
            Throw.If(table, "table").IsNull();
            Throw.If(writer, "writer").IsNull();

            string msg = string.Format("Scripting {0} indexes", table.Name);
            messageManager.OnScriptMessage(msg);

            SqlScript script = new SqlScript();
            foreach (IIndex index in table.Indexes)
            {
                msg = string.Format("Scripting index {0}", index.Name);
                messageManager.OnScriptMessage(msg);

                script += scriptBuilder.Create(index);
            }

            writer.WriteIndexScript(table.Name, script.ToScript());
        }

        /// <summary>
        /// Generates table data inserts and updates to sync two tables in
        /// different databases.
        /// </summary>
        /// <param name="source">The source table to script all data from.</param>
        /// <param name="writer">The script writer strategy.</param>
        protected virtual void ScriptTableData(ITable source, IScriptWriter writer)
        {
            Throw.If(source, "source").IsNull();
            Throw.If(writer, "writer").IsNull();

            messageManager.OnScriptMessage(
                string.Format("Starting table data scripting on table {0}.",
                              source.Name));

            script = new SqlScript();

            DataMigrator migrator = new DataMigrator();
            script = migrator.ScriptAllData(source, script);
            writer.WriteTableDataScript(source.Name, script.ToScript());

            messageManager.OnScriptMessage(
                string.Format("Finished table data scripting on table {0}.",
                              source.Name));
        }

        protected void ScriptView(IView view, IScriptWriter writer)
        {
            Throw.If(view, "view").IsNull();
            Throw.If(writer, "writer").IsNull();

            string msg = string.Format("Scripting view {0}", view.Name);
            messageManager.OnScriptMessage(msg);

            SqlScript script = scriptBuilder.Create(view);
            writer.WriteViewScript(view.Name, script.ToScript());
        }

        protected void ScriptSproc(IProcedure sproc, IScriptWriter writer)
        {
            Throw.If(sproc, "sproc").IsNull();
            Throw.If(writer, "writer").IsNull();

            string msg = string.Format("Scripting stored procedure {0}", sproc.Name);
            messageManager.OnScriptMessage(msg);

            SqlScript script = scriptBuilder.Create(sproc);
            writer.WriteSprocScript(sproc.Name, script.ToScript());
        }

        #endregion

        #region Script Differences

        // TODO: This stuff probably needs to be refactored to use IScriptWriter etc.

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

        #endregion
    }
}