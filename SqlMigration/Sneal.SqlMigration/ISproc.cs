namespace Sneal.SqlMigration
{
    public interface ISproc
    {
        string Definition { get; }

        string Schema { get; }
        string Name { get; }
    }
}
