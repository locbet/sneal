namespace Sneal.SqlMigration
{
    /// <summary>
    /// Info used to connection to a database.
    /// </summary>
    public interface IDatabaseConnectionInfo
    {
        string Name { get; }
        string Instance { get; }
        string Server { get; }
        string Port { get; }

        bool UseTrusedConnection { get; }

        string UserName { get; }
        string Password { get; }
    }
}