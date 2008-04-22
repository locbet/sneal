using MyMeta;
using Sneal.Preconditions;

namespace Sneal.SqlMigration.Utils
{
    public static class MyMetaUtil
    {
        /// <summary>
        /// Returns the ITable reference in the specified database using
        /// the table's short name.  If the table does not exist, a
        /// SqlMigrationException is thrown.
        /// </summary>
        /// <param name="db">The database to find the table in.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The table reference, this is never null.</returns>
        public static ITable GetTableOrThrow(IDatabase db, DbObjectName tableName)
        {
            Throw.If(db).IsNull();
            Throw.If(tableName).IsNull();

            ITable table = db.Tables[tableName.ShortName];
            if (table == null)
            {
                string msg = string.Format(
                    "Could not find the table {0} in the database {1}",
                    db.Name, tableName);
                throw new SqlMigrationException(msg);
            }

            return table;
        }
    }
}