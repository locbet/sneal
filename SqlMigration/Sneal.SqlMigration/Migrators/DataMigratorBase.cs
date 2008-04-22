using System;
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
        private ITable source;
        private DbObjectName sourceName;
        protected TableData tableData = new TableData();

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

        protected virtual string ScriptInsertRow(DataRow row)
        {
            Throw.If(row, "row").IsNull();

            StringBuilder values = new StringBuilder();
            int count = 0;

            foreach (IColumn col in Source.Columns)
            {
                if (col.IsComputed)
                    continue;

                if (count > 0)
                    values.Append(", ");

                values.Append(tableData.GetSqlColumnValue(col, row));

                count++;
            }

            string fmt = string.Format(
                "INSERT INTO {0} ({1}) VALUES ({2})", SourceName, GetCommaDelimitedColumnList(), "{0}");
            return string.Format(fmt, values);
        }

        private string GetCommaDelimitedColumnList()
        {
            StringBuilder select = new StringBuilder();
            int count = 0;
            foreach (IColumn col in Source.Columns)
            {
                if (col.IsComputed)
                    continue;

                if (count > 0)
                    select.Append(", ");

                select.AppendFormat("[{0}]", col.Name);

                count++;
            }

            return select.ToString();
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