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
            Throw.If(source, "source").IsNull();
            source = sourceTable;

            BuildUpdatableColumnList();
            bool hasIdentityCol = HasIdentityColumn(source);

            if (hasIdentityCol)
                script += "SET IDENTITY_INSERT [" + source.Name + "] ON\r\n\r\n";

            DataTable sourceDataTable = GetTableData(source);
            foreach (DataRow sourceRow in sourceDataTable.Rows)
            {
                script += ScriptInsertRow(sourceRow);
            }

            if (hasIdentityCol)
                script += "\r\n\r\nSET IDENTITY_INSERT [" + source.Name + "] OFF";

            return script;
        }

        protected void BuildUpdatableColumnList()
        {
            updatableColumnList.Clear();
            foreach (IColumn col in source.Columns)
            {
                if (string.Compare(col.DataTypeName, "timestamp",
                                   StringComparison.InvariantCultureIgnoreCase) != 0)
                {
                    updatableColumnList.Add(col.Name);
                }
            }
        }
    }
}