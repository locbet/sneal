using System;
using System.Data;
using System.Data.SqlClient;
using MyMeta;

namespace Sneal.SqlMigration.Impl
{
    public class DatabaseConnectionFactory
    {
        public static IDatabase CreateDbConnection(IConnectionSettings connectionInfo)
        {
            dbRoot server = new dbRoot();
            if (!server.Connect(connectionInfo.DriverType, connectionInfo.ConnectionString))
            {
                throw new SqlMigrationException("Could not connect to database server.");
            }

            IDatabase db = server.Databases[connectionInfo.Database];
            if (db == null)
            {
                throw new SqlMigrationException(
                    string.Format("Unable to find the database {0} on server.",
                                  connectionInfo.Database));
            }
            return db;
        }

        public static IDbConnection CreateDbConnection(IDatabase db)
        {
            try
            {
                return db.Root.BuildConnection(db.Root.Driver.ToString().ToUpperInvariant(),
                                               db.Root.ConnectionString);
            }
            catch (Exception)
            {
                throw new SqlMigrationException("Unable to connect to the database.");                
            }
        }

        public static IDbDataAdapter CreateDbAdapter(IDatabase db)
        {
            if (db.Root.Driver == dbDriver.SQL)
            {
                return new SqlDataAdapter();
            }

            throw new NotSupportedException(
                string.Format("{0} is not currently supported.", db.Root.Driver));
        }
    }
}