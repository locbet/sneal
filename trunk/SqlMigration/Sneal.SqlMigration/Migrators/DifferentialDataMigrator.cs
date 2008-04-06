using System;
using System.Data;
using System.Diagnostics;
using System.Text;
using MyMeta;
using Sneal.Preconditions;

namespace Sneal.SqlMigration.Migrators
{
    public class DifferentialDataMigrator : DataMigrator
    {
        private ITable target;

        protected ITable Target
        {
            get { return target; }
            set { target = value; }
        }

        public virtual SqlScript ScriptDataDifferences(ITable sourceTable, ITable targetTable, SqlScript script)
        {
            Throw.If(sourceTable, "source").IsNull();
            Throw.If(targetTable, "target").IsNull();
            Throw.If(script, "script").IsNull();

            Source = sourceTable;
            Target = targetTable;

            BuildNonComputedColumnList();
            bool hasIdentityCol = HasIdentityColumn(Source);

            DataTable sourceDataTable = GetTableData(Source);
            DataTable targetDataTable = GetTableData(Target);

            if (hasIdentityCol)
                script += "SET IDENTITY_INSERT [" + SourceName + "] ON\r\n\r\n";

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
                script += "\r\n\r\nSET IDENTITY_INSERT [" + SourceName + "] OFF";

            return script;
        }

        private string GetPrimaryKeyWhereClause(DataRow curRow)
        {
            Debug.Assert(curRow != null);

            StringBuilder pkWhere = new StringBuilder();

            if (Source.PrimaryKeys.Count == 0)
                throw new NotSupportedException(
                    "Cannot script data differences on tables without a primary key.  Submit a patch.");

            int colCount = 0;
            foreach (IColumn col in Source.PrimaryKeys)
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
            Throw.If(targetRow, "targetRow").IsNull();
            Throw.If(sourceRow, "sourceRow").IsNull();

            foreach (string columnName in nonComputedColumnList)
            {
                if (targetRow[columnName] != sourceRow[columnName])
                    return false;
            }

            return true;
        }
    }
}