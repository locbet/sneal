using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using MyMeta;
using Sneal.Preconditions;
using Sneal.SqlMigration.Utils;

namespace Sneal.SqlMigration.Impl
{
    public class TableData
    {
        public int QueryTimeoutInSeconds = 120;

        public virtual DataTable GetTableData(ITable table)
        {
            Throw.If(table, "table").IsNull();

            using (IDbConnection tableConnection = DatabaseConnectionFactory.CreateDbConnection(table.Database))
            {
                try
                {
                    tableConnection.Open();
                }
                catch (DbException ex)
                {
                    string msg = string.Format("The data migrator could not connect to the database {0}",
                                               table.Database.Name);
                    throw new SqlMigrationException(msg, ex);
                }

                IDbCommand cmd = tableConnection.CreateCommand();
                cmd.CommandText = BuildTableSelectStatement(table);
                cmd.CommandTimeout = QueryTimeoutInSeconds;
                cmd.CommandType = CommandType.Text;

                IDbDataAdapter adapter = DatabaseConnectionFactory.CreateDbAdapter(table.Database);
                adapter.SelectCommand = cmd;
                DataSet ds = new DataSet();

                try
                {
                    adapter.Fill(ds);
                }
                catch (DbException ex)
                {
                    string msg = string.Format("The data migrator could not query the table {0} in database {1}",
                                               table.Name,
                                               table.Database.Name);
                    throw new SqlMigrationException(msg, ex);
                }

                Debug.Assert(ds.Tables.Count == 1);
                return ds.Tables[0];
            }
        }

        public virtual string BuildTableSelectStatement(ITable table)
        {
            Throw.If(table).IsNull();

            List<string> columns = new List<string>();
            foreach (IColumn col in table.Columns)
            {
                if (!col.IsComputed)
                    columns.Add("[" + col.Name + "]");
            }

            string colList = string.Join(", ", columns.ToArray());
            return string.Format("SELECT {0} FROM {1}", colList, table.Name);
        }

        /// <summary>
        /// Gets the SQL value ready for insert for this row and column.
        /// </summary>
        /// <param name="col">The column to retrieve the value.</param>
        /// <param name="row">The current row.</param>
        /// <returns>The escaped value.</returns>
        public virtual string GetSqlColumnValue(IColumn col, DataRow row)
        {
            Throw.If(col, "col").IsNull();
            Throw.If(row, "row").IsNull();

            if (col.IsNullable && row[col.Name] == DBNull.Value)
            {
                return "NULL";
            }
            else if (DataTypeUtil.IsNumeric(col))
            {
                return row[col.Name].ToString();
            }
            else if (DataTypeUtil.IsBoolean(col))
            {
                if ((bool)row[col.Name])
                    return "1";
                else
                    return "0";
            }
            else
            {
                return string.Format("'{0}'", row[col.Name].ToString().Replace("'", "''"));
            }
        }

        public virtual string GetXmlColumnValue(IColumn col, DataRow row)
        {
            Throw.If(col, "col").IsNull();
            Throw.If(row, "row").IsNull();

            string val = GetSqlColumnValue(col, row);
            if (val.StartsWith("'") && val.EndsWith("'"))
                val = val.Substring(1, val.Length - 2);

            return val;            
        }
    }
}