using System.Collections.Generic;

namespace Sneal.SqlMigration
{
    /// <summary>
    /// This interface encapuslates a database, including its tables
    /// sprocs, and views.
    /// </summary>
    public interface IDatabase
    {
        IList<ITable> Tables { get; }
        IList<IView> Views { get; }
        IList<ISproc> Sprocs { get; }

        /// <summary>
        /// The short name of the database.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Returns <code>true</code> if this database is the target database
        /// in which changes are derived from.
        /// </summary>
        bool IsLatestVersion { get; set; }
    }
}