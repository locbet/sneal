using System;
using System.Data;
using MyMeta;
using Sneal.Preconditions;

namespace Sneal.SqlMigration.Migrators
{
    public class DataMigrator : DataMigratorBase
    {
        public virtual SqlScript ScriptAllData(ITable sourceTable, SqlScript script)
        {
            Throw.If(sourceTable, "sourceTable").IsNull();
            Throw.If(script, "script").IsNull();

            Source = sourceTable;

            BuildNonComputedColumnList();
            bool hasIdentityCol = HasIdentityColumn(Source);

            if (hasIdentityCol)
                script += "SET IDENTITY_INSERT [" + SourceName + "] ON\r\n\r\n";

            DataTable sourceDataTable = GetTableData(Source);
            foreach (DataRow sourceRow in sourceDataTable.Rows)
            {
                script += ScriptInsertRow(sourceRow) + "\r\n";
            }

            if (hasIdentityCol)
                script += "\r\n\r\nSET IDENTITY_INSERT [" + SourceName + "] OFF\r\n";

            return script;
        }

        protected void BuildNonComputedColumnList()
        {
            nonComputedColumnList.Clear();
            foreach (IColumn col in Source.Columns)
            {
                if (!col.IsComputed)
                    nonComputedColumnList.Add(col.Name);
            }
        }
    }
}