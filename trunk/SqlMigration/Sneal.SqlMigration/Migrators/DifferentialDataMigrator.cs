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

            DataTable sourceDataTable = GetTableData(Source);
            DataTable targetDataTable = GetTableData(Target);

            bool scriptedInsert = false;
            SqlScript dataScript = new SqlScript();

            foreach (DataRow sourceRow in sourceDataTable.Rows)
            {
                string whereClause = GetPrimaryKeyWhereClause(sourceRow);
                DataRow[] targetRows = targetDataTable.Select(whereClause);
                Debug.Assert(targetRows.Length <= 1);

                if (targetRows.Length == 0)
                {
                    // data doesn't exist in target, so script it
                    dataScript += ScriptInsertRow(sourceRow) + "\r\n";
                    scriptedInsert = true;
                }
                else if (!DataRowIsEqual(targetRows[0], sourceRow))
                {
                    // pk exists, but one or more columns differ.
                    dataScript += ScriptUpdateRow(sourceRow) + "\r\n";
                }
            }

            if (HasIdentityColumn(sourceTable) && scriptedInsert)
            {
                script += "SET IDENTITY_INSERT " + SourceName + " ON\r\n\r\n";
                script += dataScript;
                script += "\r\nSET IDENTITY_INSERT " + SourceName + " OFF";
            }
            else
            {
                script += dataScript;
            }

            return script;
        }

        protected virtual string ScriptUpdateRow(DataRow row)
        {
            Throw.If(row, "row").IsNull();

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("UPDATE {0} SET ", SourceName);

            int count = 0;
            foreach (IColumn col in Source.Columns)
            {
                if (col.IsComputed || col.IsInPrimaryKey)
                    continue;

                if (count > 0)
                    sb.Append(", ");

                sb.Append(col.Name);
                sb.Append(" = ");
                sb.Append(GetColumnValue(col, row));

                count++;
            }

            string whereClause = GetPrimaryKeyWhereClause(row);
            sb.Append(" WHERE ").Append(whereClause);

            return sb.ToString();
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

            foreach (IColumn col in Source.Columns)
            {
                object val1 = targetRow[col.Name];
                object val2 = sourceRow[col.Name];

                // need to use overridden Equals() for value types
                if (!val1.Equals(val2))
                    return false;
            }

            return true;
        }
    }
}