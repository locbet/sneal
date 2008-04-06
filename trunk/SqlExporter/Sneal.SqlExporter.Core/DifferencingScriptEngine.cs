using System;
using System.Collections.Generic;
using System.Text;
using Sneal.Preconditions;
using SqlAdmin;

namespace Sneal.SqlExporter.Core
{
    public class DifferencingScriptEngine : ScriptEngine
    {
        private readonly SqlDatabase targetDatabase;

        public DifferencingScriptEngine(SqlDatabase database, SqlDatabase targetDatabase)
            : base(database)
        {
            Throw.If(targetDatabase).IsNull();
            this.targetDatabase = targetDatabase;
        }

        /// <summary>
        /// Exports the T-SQL scripts to disk.
        /// </summary>
        public virtual void ExportUpgradeScripts(IExportParams exportParameters, IScriptWriter scriptWriter)
        {
            Throw.If(exportParameters).IsNull();
            Throw.If(scriptWriter).IsNull();

            exportParams = exportParameters;
            writer = scriptWriter;

            CalculateScriptObjectCount();

            ScriptTableDataDifferences();
        }

        protected void ScriptTableDataDifferences()
        {
            if (!exportParams.ScriptDataAsSql)
                return;

            foreach (string tableName in exportParams.TablesToScriptData)
            {
                SqlTable table = database.Tables[tableName];
                if (table == null)
                {
                    string msg = string.Format(
                        "Cannot find the table {0} in the source database {1}",
                        tableName, database.Name);
                    throw new SqlExporterException(msg);
                }

                SqlTable targetTable = targetDatabase.Tables[tableName];
                if (targetTable == null)
                {
                    string msg = string.Format(
                        "Cannot find the table {0} in the target database {1}",
                        tableName, targetDatabase.Name);
                    throw new SqlExporterException(msg);
                }

                // get table PK
            }
        }
    }
}
