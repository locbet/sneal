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
            sql.AppendFormat("[{0}] ", column.DataTypeNameComplete);
            if (!column.IsNullable)
                sql.Append("NOT ");
            sql.Append("NULL");

            return sql.ToString();
        }
    }
}
