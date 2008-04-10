using System.Text;
using MyMeta;

namespace Sneal.SqlMigration.Impl
{
    /// <summary>
    /// Helper class used by NVelocity templates to help format SQL output.
    /// </summary>
    public class SqlServerScriptHelper
    {
        public string WriteColumn(IColumn column)
        {
            StringBuilder sql = new StringBuilder(50);
            sql.AppendFormat("[{0}] ", column.Name);
            sql.AppendFormat("{0}", column.DataTypeNameComplete);

            if (column.IsAutoKey)
                sql.AppendFormat(" IDENTITY({0},{1})", column.AutoKeySeed, column.AutoKeyIncrement);

            if (!column.IsNullable)
                sql.Append(" NOT");

            sql.Append(" NULL");

            if (column.HasDefault)
                sql.AppendFormat(" DEFAULT {0}", column.Default);

            return sql.ToString();
        }
    }
}
