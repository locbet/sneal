namespace Sneal.SqlMigration
{
    public interface IIndex
    {
        IColumn Column { get; }
        bool IsUnique { get; }

        string Schema { get; }
        string Name { get; }
    }
}