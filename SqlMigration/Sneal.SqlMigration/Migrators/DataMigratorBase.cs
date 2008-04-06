using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Text;
using MyMeta;
using Sneal.Preconditions;
using Sneal.SqlMigration.Impl;
using Sneal.SqlMigration.Utils;

namespace Sneal.SqlMigration.Migrators
{
    public abstract class DataMigratorBase
    {
        public int QueryTimeout = 120;
        protected readonly List<string> nonComputedColumnList = new List<string>();
        private ITable source;
        private DbObjectName sourceName;

        protected ITable Source
        {
            get { return source; }
            set
            {
                source = value;
                sourceName = DbObjectName.CreateDbObjectName(source);
            }
        }

        public DbObjectName SourceName
        {
            get { return sourceName; }
        }

        protected virtual DataTable GetTableData(ITable table)
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
                cmd.CommandText = GetSelectClause();
                cmd.CommandTimeout = QueryTimeout;
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

        protected virtual string ScriptUpdateRow(DataRow row)
        {
            Throw.If(row, "row").IsNull();

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("UPDATE {0} SET ", SourceName);

            int count = 0;
            foreach (string columnName in nonComputedColumnList)
            {
                if (count > 0)
                    sb.Append(", ");

                IColumn col = source.Columns[columnName];

                sb.Append(columnName);
                sb.Append(" = ");
                sb.Append(GetColumnValue(col, row));

                count++;
            }

            return sb.ToString();
        }

        protected virtual string ScriptInsertRow(DataRow row)
        {
            Throw.If(row, "row").IsNull();

            StringBuilder values = new StringBuilder();
            int count = 0;

            foreach (string columnName in nonComputedColumnList)
            {
                if (count > 0)
                    values.Append(", ");

                IColumn col = source.Columns[columnName];

                values.Append(GetColumnValue(col, row));

                count++;
            }

            string fmt = string.Format(
                "INSERT INTO {0} ({1}) VALUES ({2})", SourceName, GetCommaDelimitedColumnList(), "{0}");
            return string.Format(fmt, values);
        }

        protected static string GetColumnValue(IColumn col, DataRow row)
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
                if ((bool) row[col.Name])
                    return "1";
                else
                    return "0";
            }
            else
            {
                return string.Format("'{0}'", row[col.Name].ToString().Replace("'", "''"));
            }
        }

        private string GetCommaDelimitedColumnList()
        {
            StringBuilder select = new StringBuilder();
            int count = 0;
            foreach (string col in nonComputedColumnList)
            {
                if (count > 0)
                    select.Append(", ");

                select.AppendFormat("[{0}]", col);

                count++;
            }

            return select.ToString();
        }

        private string GetSelectClause()
        {
            return string.Format("SELECT {0} FROM {1}", GetCommaDelimitedColumnList(), SourceName);
        }

        protected static bool HasIdentityColumn(ITable table)
        {
            Throw.If(table, "table").IsNull();

            foreach (IColumn col in table.Columns)
            {
                if (col.IsAutoKey)
                    return true;
            }

            return false;
        }
    }
}