using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;
using MyMeta;
using Sneal.Preconditions;

namespace Sneal.SqlMigration.Migrators
{
    public class DifferentialDataMigrator : DataMigrator
    {
        private readonly List<string> pkColumnList = new List<string>();
        private ITable target;

        public virtual SqlScript ScriptDataDifferences(ITable sourceTable, ITable targetTable, SqlScript script)
        {
            Throw.If(source, "source").IsNull();
            Throw.If(target, "target").IsNull();

            source = sourceTable;
            target = targetTable;

            BuildUpdatableColumnList();
            BuildPkColumnList();
            bool hasIdentityCol = HasIdentityColumn(source);

            DataTable sourceDataTable = GetTableData(source);
            DataTable targetDataTable = GetTableData(target);

            if (hasIdentityCol)
                script += "SET IDENTITY_INSERT [" + source.Name + "] ON\r\n\r\n";

            foreach (DataRow sourceRow in sourceDataTable.Rows)
            {
                DataRow[] targetRows = targetDataTable.Select(GetPrimaryKeyWhereClause(sourceRow));
                Debug.Assert(targetRows.Length <= 1);

                if (targetRows.Length == 0)
                {
                    // data doesn't exist in target, so script it
                    script += ScriptInsertRow(sourceRow);
                }
                else if (!DataRowIsEqual(targetRows[0], sourceRow))
                {
                    // pk exists, but other columns differ.
                    script += ScriptUpdateRow(sourceRow);
                }
            }

            if (hasIdentityCol)
                script += "\r\n\r\nSET IDENTITY_INSERT [" + source.Name + "] OFF";

            return script;
        }

        private void BuildPkColumnList()
        {
            pkColumnList.Clear();
            foreach (IColumn col in source.PrimaryKeys)
            {
                pkColumnList.Add(col.Name);
            }
        }

        private string GetPrimaryKeyWhereClause(DataRow curRow)
        {
            StringBuilder pkWhere = new StringBuilder();

            if (source.PrimaryKeys.Count == 0)
                throw new NotSupportedException(
                    "Cannot script data differences on tables without a primary key.  Submit a patch.");

            int colCount = 0;
            foreach (IColumn col in source.PrimaryKeys)
            {
                if (colCount > 0)
                    pkWhere.Append(" AND ");

                pkWhere.AppendFormat("{0} = {1}", col.Name, curRow[col.Name]);

                colCount++;
            }

            return pkWhere.ToString();
        }

        protected virtual bool DataRowIsEqual(DataRow targetRow, DataRow sourceRow)
        {
            foreach (string columnName in updatableColumnList)
            {
                if (targetRow[columnName] != sourceRow[columnName])
                    return false;
            }

            return true;
        }
    }
}