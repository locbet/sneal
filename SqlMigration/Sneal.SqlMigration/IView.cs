namespace Sneal.SqlMigration
{
    public interface IView
    {
        string Definition { get; }

        string Schema { get; }
        string Name { get; }
    }
}
