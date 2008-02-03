using System.Text;

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
            sql.AppendFormat("[{0}] ", column.DataType.Name);
            if (column.DataType.Length > 0)
                sql.AppendFormat("({0}) ", column.DataType.Length);
            if (!column.IsNullable)
                sql.Append("NOT ");
            sql.Append("NULL");

            return sql.ToString();
        }
    }
}
