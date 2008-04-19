using System;
using System.Collections.Generic;
using MyMeta;
using Sneal.Preconditions;
using Sneal.SqlMigration.Impl;
using Sneal.SqlMigration.Migrators;
using Sneal.SqlMigration.Writers;

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
        public MigrationEngine(IScriptBuilder scriptBuilder)
            : this(scriptBuilder, new DatabaseComparer()) { }

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

            messageManager.OnScriptMessage("Starting database scripting.");

            IDatabase db = DatabaseConnectionFactory.CreateDbConnection(connectionInfo);

            List<ITable> tablesToScript = new List<ITable>();
            foreach (DbObjectName tableName in scriptingOptions.TablesToScript)
            {
                ITable table = db.Tables[tableName.ShortName];
                if (table == null)
                {
                    throw new SqlMigrationException(
                        string.Format("Unable to find the table {0} in database {1} on server.",
                                      tableName, connectionInfo.Database));
                }

                tablesToScript.Add(table);
            }

            IScriptWriter writer = CreateScriptWriter(scriptingOptions, connectionInfo);
            int totalObjects = CalculateScriptObjectCount(scriptingOptions);

            int exportCount = 0;
            if (scriptingOptions.ScriptSchema)
            {
                foreach (ITable table in tablesToScript)
                {
                    ScriptTableSchema(table, writer);
                    OnProgressEvent(++exportCount, totalObjects);
                }
            }

            if (scriptingOptions.ScriptIndexes)
            {
                foreach (ITable table in tablesToScript)
                {
                    ScriptTableIndexes(table, writer);
                    OnProgressEvent(++exportCount, totalObjects);
                }
            }

            if (scriptingOptions.ScriptData || scriptingOptions.ScriptDataAsXml)
            {
                IScriptWriter dataWriter = writer;
                IDataMigrator dataMigrator = new DataMigrator();
                if (scriptingOptions.ScriptDataAsXml)
                {
                    dataMigrator = new XmlDataMigrator();
                    dataWriter = new XmlDataWriter(scriptingOptions.ExportDirectory, messageManager);
                }

                foreach (ITable table in tablesToScript)
                {
                    ScriptTableData(dataMigrator, table, dataWriter);
                    OnProgressEvent(++exportCount, totalObjects);
                }
            }

            if (scriptingOptions.ScriptForeignKeys)
            {
                foreach (ITable table in tablesToScript)
                {
                    ScriptTableForeignKeys(table, writer);
                    OnProgressEvent(++exportCount, totalObjects);
                }
            }

            foreach (DbObjectName sprocName in scriptingOptions.SprocsToScript)
            {
                IProcedure sproc = db.Procedures[sprocName.ShortName];
                if (sproc == null)
                {
                    throw new SqlMigrationException(
                        string.Format("Unable to find the procedure {0} in database {1} on server.",
                                      sprocName, connectionInfo.Database));
                }
                ScriptSproc(sproc, writer);
                OnProgressEvent(++exportCount, totalObjects);
            }

            foreach (DbObjectName viewName in scriptingOptions.ViewsToScript)
            {
                IView view = db.Views[viewName.ShortName];
                if (view == null)
                {
                    throw new SqlMigrationException(
                        string.Format("Unable to find the view {0} in database {1} on server.",
                                      viewName, connectionInfo.Database));
                }
                ScriptView(view, writer);
                OnProgressEvent(++exportCount, totalObjects);
            }

            messageManager.OnScriptMessage("Finished database scripting.");
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
            Throw.If(connectionInfoSourceDb, "connectionInfoSourceDb").IsNull();
            Throw.If(connectionInfoTargetDb, "connectionInfoTargetDb").IsNull();
            Throw.If(scriptingOptions, "scriptingOptions").IsNull();

            messageManager.OnScriptMessage("Starting scripting database differences.");

            IDatabase srcDb = DatabaseConnectionFactory.CreateDbConnection(connectionInfoSourceDb);
            IDatabase targetDb = DatabaseConnectionFactory.CreateDbConnection(connectionInfoTargetDb);

            IScriptWriter writer = CreateScriptWriter(scriptingOptions, connectionInfoTargetDb);
            int totalObjects = CalculateScriptObjectCount(scriptingOptions);

            int exportCount = 0;
            foreach (DbObjectName tableName in scriptingOptions.TablesToScript)
            {
                // TODO: Need to split this up like the Script() method to order things correctly?
                ITable table = srcDb.Tables[tableName.ShortName];
                if (table == null)
                {
                    throw new SqlMigrationException(
                        string.Format("Unable to find the source table {0} in database {1} on server.",
                                      tableName, connectionInfoSourceDb.Database));
                }

                ITable targetTable = targetDb.Tables[tableName.ShortName];
                if (targetTable == null)
                {
                    throw new SqlMigrationException(
                        string.Format("Unable to find the target table {0} in database {1} on server.",
                                      tableName, connectionInfoTargetDb.Database));
                }

                if (scriptingOptions.ScriptData)
                {
                    ScriptTableDataDifferences(table, targetTable, writer);
                    OnProgressEvent(++exportCount, totalObjects);
                }

                // TODO: constraints, Schema, Indexes?
            }

            messageManager.OnScriptMessage("Finished scripting database differences.");

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

        private IScriptWriter CreateScriptWriter(IScriptingOptions scriptingOptions, IConnectionSettings connSettings)
        {
            IScriptWriter writer;
            if (scriptingOptions.UseMultipleFiles)
                writer = new MultiFileScriptWriter(scriptingOptions.ExportDirectory);
            else
                writer = new SingleFileScriptWriter(scriptingOptions.ExportDirectory, connSettings.Database);

            writer.MessageManager = messageManager;
            return writer;
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
            //tableWeight += options.ScriptForeignKeys ? 1 : 0;
            tableWeight += options.ScriptData ? 1 : 0;

            int exportObjectTotal = options.TablesToScript.Count*tableWeight;
            exportObjectTotal += options.ViewsToScript.Count;
            exportObjectTotal += options.SprocsToScript.Count;

            return exportObjectTotal;
        }

        #region List Objects

        public IList<DbObjectName> GetAllTables(IConnectionSettings connectionSettings)
        {
            List<DbObjectName> tableNames = new List<DbObjectName>();

            IDatabase db = DatabaseConnectionFactory.CreateDbConnection(connectionSettings);
            foreach (ITable table in db.Tables)
            {
                tableNames.Add(table.Name);
            }

            return tableNames;
        }

        public IList<DbObjectName> GetAllViews(IConnectionSettings connectionSettings)
        {
            List<DbObjectName> viewNames = new List<DbObjectName>();

            IDatabase db = DatabaseConnectionFactory.CreateDbConnection(connectionSettings);
            foreach (IView view in db.Views)
            {
                viewNames.Add(view.Name);
            }

            return viewNames;
        }

        public IList<DbObjectName> GetAllSprocs(IConnectionSettings connectionSettings)
        {
            List<DbObjectName> sprocNames = new List<DbObjectName>();

            IDatabase db = DatabaseConnectionFactory.CreateDbConnection(connectionSettings);
            foreach (IProcedure sproc in db.Procedures)
            {
                if (sproc.Schema != "sys")
                    sprocNames.Add(sproc.Name);
            }

            return sprocNames;
        }

        #endregion

        #region Script All Objects

        protected void ScriptTableSchema(ITable table, IScriptWriter writer)
        {
            Throw.If(table, "table").IsNull();
            Throw.If(writer, "writer").IsNull();

            string tableName = DbObjectName.CreateDbObjectName(table);

            string msg = string.Format("Scripting {0} table schema", tableName);
            messageManager.OnScriptMessage(msg);

            SqlScript script = scriptBuilder.Create(table);
            writer.WriteTableScript(tableName, script.ToScript());
        }

        protected void ScriptTableIndexes(ITable table, IScriptWriter writer)
        {
            Throw.If(table, "table").IsNull();
            Throw.If(writer, "writer").IsNull();

            string tableName = DbObjectName.CreateDbObjectName(table);

            string msg = string.Format("Scripting {0} indexes", tableName);
            messageManager.OnScriptMessage(msg);

            SqlScript script = new SqlScript();
            foreach (IIndex index in table.Indexes)
            {
                msg = string.Format("Scripting index {0}", index.Name);
                messageManager.OnScriptMessage(msg);

                script += scriptBuilder.Create(index);
            }

            writer.WriteIndexScript(tableName, script.ToScript());
        }

        protected virtual void ScriptTableForeignKeys(ITable table, IScriptWriter writer)
        {
            Throw.If(table, "table").IsNull();
            Throw.If(writer, "writer").IsNull();

            string tableName = DbObjectName.CreateDbObjectName(table);

            string msg = string.Format("Scripting {0} foreign keys", tableName);
            messageManager.OnScriptMessage(msg);

            SqlScript script = new SqlScript();
            foreach (IForeignKey fk in table.ForeignKeys)
            {
                // only script fks on fk table
                if (fk.PrimaryTable == table)
                    continue;

                msg = string.Format("Scripting foreign key {0}", fk.Name);
                messageManager.OnScriptMessage(msg);

                script += scriptBuilder.Create(fk);
            }

            writer.WriteForeignKeyScript(tableName, script.ToScript());            
        }

        /// <summary>
        /// Generates table data inserts and updates to sync two tables in
        /// different databases.
        /// </summary>
        /// <param name="migrator">The data migrator instance.</param>
        /// <param name="source">The source table to script all data from.</param>
        /// <param name="writer">The script writer strategy.</param>
        protected virtual void ScriptTableData(IDataMigrator migrator, ITable source, IScriptWriter writer)
        {
            Throw.If(source, "source").IsNull();
            Throw.If(writer, "writer").IsNull();

            string name = DbObjectName.CreateDbObjectName(source);

            messageManager.OnScriptMessage(
                string.Format("Starting table data scripting on table {0}.",
                              name));

            script = new SqlScript();

            // TODO: What about XML, do we use a different writer?
            script = migrator.ScriptAllData(source, script);
            writer.WriteTableDataScript(name, script.ToScript());

            messageManager.OnScriptMessage(
                string.Format("Finished table data scripting on table {0}.",
                              name));
        }

        protected void ScriptView(IView view, IScriptWriter writer)
        {
            Throw.If(view, "view").IsNull();
            Throw.If(writer, "writer").IsNull();

            string name = DbObjectName.CreateDbObjectName(view);

            string msg = string.Format("Scripting view {0}", name);
            messageManager.OnScriptMessage(msg);

            SqlScript script = scriptBuilder.Create(view);
            writer.WriteViewScript(name, script.ToScript());
        }

        protected void ScriptSproc(IProcedure sproc, IScriptWriter writer)
        {
            Throw.If(sproc, "sproc").IsNull();
            Throw.If(writer, "writer").IsNull();

            string name = DbObjectName.CreateDbObjectName(sproc);

            string msg = string.Format("Scripting stored procedure {0}", name);
            messageManager.OnScriptMessage(msg);

            SqlScript script = scriptBuilder.Create(sproc);
            writer.WriteSprocScript(name, script.ToScript());
        }

        #endregion

        #region Script Differences

        protected virtual void ScriptTableDataDifferences(ITable sourceTable, ITable targetTable, IScriptWriter writer)
        {
            Throw.If(sourceTable, "sourceTable").IsNull();
            Throw.If(targetTable, "targetTable").IsNull();
            Throw.If(writer, "writer").IsNull();

            string name = DbObjectName.CreateDbObjectName(sourceTable);

            messageManager.OnScriptMessage(
                string.Format("Starting table data difference scripting on table {0}.",
                              name));

            script = new SqlScript();

            DifferentialDataMigrator migrator = new DifferentialDataMigrator();
            script = migrator.ScriptDataDifferences(sourceTable, targetTable, script);
            writer.WriteTableDataScript(name, script.ToScript());

            messageManager.OnScriptMessage(
                string.Format("Finished table data difference scripting on table {0}.",
                              name));            
        }

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