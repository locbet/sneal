using System;
using System.Collections.Generic;
using System.Text;

namespace Sneal.SqlMigration
{
    /// <summary>
    /// Implmentors should load the schema of the specified database.
    /// </summary>
    public interface IDatabaseLoader
    {
        IDatabase LoadDatabase(IDatabaseConnectionInfo connection);
    }
}
