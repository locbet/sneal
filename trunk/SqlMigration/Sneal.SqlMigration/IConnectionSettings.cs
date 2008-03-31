namespace Sneal.SqlMigration
{
    public interface IConnectionSettings
    {
        /// <summary>
        /// Corresponds to the MyMeta Driver type.
        /// </summary>
        string DriverType { get; }

        /// <summary>
        /// The RDMS connection string.
        /// </summary>
        string ConnectionString { get; }

        /// <summary>
        /// The database name to connect to.
        /// </summary>
        string Database { get; }
    }
}