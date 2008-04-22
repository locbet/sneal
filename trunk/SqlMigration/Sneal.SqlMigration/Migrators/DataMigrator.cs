using System.Data;
using MyMeta;
using Sneal.Preconditions;

namespace Sneal.SqlMigration.Migrators
{
    public class DataMigrator : DataMigratorBase, IDataMigrator
    {
        #region IDataMigrator Members

        public virtual SqlScript ScriptAllData(ITable sourceTable, SqlScript script)
        {
            Throw.If(sourceTable, "sourceTable").IsNull();
            Throw.If(script, "script").IsNull();

            Source = sourceTable;

            DataTable sourceDataTable = tableData.GetTableData(Source);
            SqlScript dataScript = new SqlScript();

            foreach (DataRow sourceRow in sourceDataTable.Rows)
            {
                dataScript += ScriptInsertRow(sourceRow) + "\r\n";
            }

            if (HasIdentityColumn(sourceTable))
            {
                script += "SET IDENTITY_INSERT [" + SourceName + "] ON\r\n";
                script += dataScript;
                script += "SET IDENTITY_INSERT [" + SourceName + "] OFF";
            }
            else
            {
                script += dataScript;
            }

            return script;
        }

        #endregion
    }
}