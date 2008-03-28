using System;
using Sneal.Preconditions;
using SqlAdmin;

namespace Sneal.SqlExporter.Core
{
    /// <summary>
    /// This scripter implmentation script out the specified objects to separate
    /// SQL script files in distinct sub-folders.  This scripter should
    /// generally be used to check in SQL script into a source control system.
    /// </summary>
    /// <remarks>
    /// This class is definately not thread safe.
    /// </remarks>
    public class ScriptEngine
    {
        private readonly SqlDatabase database;
        private int exportObjectCount;
        private int exportObjectTotal;
        private IExportParams exportParams;

        /// <summary>
        /// This event fires each time a script is written to disk.
        /// </summary>
        public event EventHandler<ProgressEventArgs> ProgressEvent;

        private IScriptWriter writer;

        public ScriptEngine(SqlDatabase database)
        {
            Throw.If(database).IsNull();
            this.database = database;
        }

        /// <summary>
        /// Exports the T-SQL scripts to disk.
        /// </summary>
        public virtual void ExportScripts(IExportParams exportParameters, IScriptWriter scriptWriter)
        {
            Throw.If(exportParameters).IsNull();
            Throw.If(scriptWriter).IsNull();

            exportParams = exportParameters;
            writer = scriptWriter;

            CalculateScriptObjectCount();

            ScriptSprocs();
            ScriptViews();
            ScriptTables();
            ScriptTableData();
        }

        protected virtual void OnProgressEvent(ProgressEventArgs e)
        {
            Throw.If(e).IsNull();

            EventHandler<ProgressEventArgs> evt = ProgressEvent;
            if (evt != null)
            {
                evt(this, e);
            }
        }

        /// <summary>
        /// Increments the progress counter by 1 and sends an OnProgressEvent.
        /// </summary>
        /// <param name="objectName">The name of the SQL object just written.</param>
        protected virtual void UpdateProgress(string objectName)
        {
            Throw.If(objectName).IsEmpty();

            exportObjectCount++;
            ushort percentDone = (ushort) (((float) exportObjectCount/exportObjectTotal)*100);

            ProgressEventArgs eventArgs = new ProgressEventArgs(percentDone, objectName);
            OnProgressEvent(eventArgs);
        }

        /// <summary>
        /// Calculates the total number of scripts that will be written.
        /// </summary>
        protected virtual void CalculateScriptObjectCount()
        {
            int tableWeight = exportParams.ScriptTableSchema ? 1 : 0;
            tableWeight += exportParams.ScriptTableIndexes ? 1 : 0;
            tableWeight += exportParams.ScriptTableConstraints ? 1 : 0;

            int dataWeight = exportParams.ScriptDataAsSql ? 1 : 0;
            dataWeight += exportParams.ScriptDataAsXml ? 1 : 0;

            exportObjectTotal = exportParams.TablesToScript.Count * tableWeight;
            exportObjectTotal += exportParams.TablesToScriptData.Count * dataWeight;
            exportObjectTotal += exportParams.ViewsToScript.Count;
            exportObjectTotal += exportParams.SprocsToScript.Count;
        }

        protected void ScriptTableData()
        {
            if (!exportParams.ScriptDataAsSql)
                return;

            foreach (string tableName in exportParams.TablesToScriptData)
            {
                SqlTable table = database.Tables[tableName];
                if (table == null)
                {
                    string msg = string.Format(
                        "Cannot find the table {0} in the database {1}",
                        tableName, database.Name);
                    throw new SqlExporterException(msg);
                }

                // get sproc script with drop statement and comments
                string sql = table.ScriptData(SqlScriptType.Comments);

                string objectName = table.Name + " Data";

                writer.WriteTableDataScript(objectName, sql);
                UpdateProgress(objectName);
            }
        }

        protected virtual void ScriptTables()
        {
            foreach (string tableName in exportParams.TablesToScript)
            {
                SqlTable table = database.Tables[tableName];
                if (table == null)
                {
                    string msg = string.Format(
                        "Cannot find the table {0} in the database {1}",
                        tableName, database.Name);
                    throw new SqlExporterException(msg);
                }

                WriteTableSchemaScript(table);
                WriteTableConstraintScript(table);
                WriteTableIndexScript(table);
            }
        }

        protected virtual void ScriptViews()
        {
            foreach (string viewName in exportParams.ViewsToScript)
            {
                // export current sproc
                SqlView view = database.Views[viewName];
                if (view == null)
                {
                    string msg = string.Format(
                        "Cannot find the view {0} in the database {1}",
                        viewName, database.Name);
                    throw new SqlExporterException(msg);
                }

                // get sproc script with drop statement and comments
                string sql = view.Script(SqlScriptType.Create |
                                         SqlScriptType.Drop |
                                         SqlScriptType.Comments |
                                         SqlScriptType.Permissions);

                writer.WriteViewScript(view.Name, sql);
                UpdateProgress(view.Name);
            }
        }

        protected void ScriptSprocs()
        {
            foreach (string sprocName in exportParams.SprocsToScript)
            {
                // export current sproc
                SqlStoredProcedure sproc = database.StoredProcedures[sprocName];
                if (sproc == null)
                {
                    string msg = string.Format(
                        "Cannot find the stored procedure {0} in the database {1}",
                        sprocName, database.Name);
                    throw new SqlExporterException(msg);
                }

                // get sproc script
                string sql = sproc.Script(
                    SqlScriptType.Create |
                    SqlScriptType.Drop |
                    SqlScriptType.Comments |
                    SqlScriptType.Permissions);

                writer.WriteSprocScript(sproc.Name, sql);
                UpdateProgress(sproc.Name);
            }
        }

        protected virtual void WriteTableSchemaScript(SqlTable table)
        {
            Throw.If(table).IsNull();

            if (!exportParams.ScriptTableSchema)
                return;

            string sql = table.ScriptSchema(SqlScriptType.Create |
                                            SqlScriptType.Drop |
                                            SqlScriptType.Defaults |
                                            SqlScriptType.Comments |
                                            SqlScriptType.Permissions |
                                            SqlScriptType.PrimaryKey);

            writer.WriteTableScript(table.Name, sql);
            UpdateProgress(table.Name);
        }

        protected virtual void WriteTableConstraintScript(SqlTable table)
        {
            Throw.If(table).IsNull();

            if (!exportParams.ScriptTableConstraints)
                return;

            string sql = table.ScriptSchema(
                SqlScriptType.Checks |
                SqlScriptType.ForeignKeys |
                SqlScriptType.UniqueKeys);

            string objectName = table.Name + " Constraints";

            writer.WriteConstraintScript(objectName, sql);
            UpdateProgress(objectName);
        }

        protected virtual void WriteTableIndexScript(SqlTable table)
        {
            Throw.If(table).IsNull();

            if (!exportParams.ScriptTableIndexes)
                return;

            string sql = table.ScriptIndexes();
            string objectName = table.Name + " Indexes";

            writer.WriteIndexScript(objectName, sql);
            UpdateProgress(objectName);
        }
    } // end class
}