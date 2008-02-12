using MyMeta;

namespace Sneal.SqlMigration
{
    /// <summary>
    /// Provides a fluent interface for DB comparisons.
    /// </summary>
    /// <remarks>
    /// <c>dbComparer.Index(index).ExistsIn(sourceDB)</c>
    /// </remarks>
    public interface IDatabaseComparer
    {
        IDatabaseComparer Table(ITable table);
        IDatabaseComparer Column(IColumn column);
        IDatabaseComparer ForeignKey(IForeignKey fk);
        IDatabaseComparer Index(IIndex index);

        bool ExistsIn(IDatabase targetDB);
    }
}