namespace Sneal.SqlMigration
{
    public interface IForeignKey
    {
        IColumn ForeignKeyColumn { get; }
        IColumn PrimaryKeyColumn { get; }

        string Schema { get; }
        string Name { get; }
    }
}