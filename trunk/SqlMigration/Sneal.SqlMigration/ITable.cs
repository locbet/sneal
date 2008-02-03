using System.Collections.Generic;

namespace Sneal.SqlMigration
{
    public interface ITable
    {
        IList<IColumn> Columns { get; }

        string Schema { get; }
        string Name { get; }
    }
}