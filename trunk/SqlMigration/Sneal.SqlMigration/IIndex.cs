using System.Collections.Generic;

namespace Sneal.SqlMigration
{
    public interface IIndex
    {
        IList<IColumn> Columns { get; }
        ITable Table { get; }

        bool IsUnique { get; }
        bool IsPrimaryKey { get; }
        bool IsClustered { get; }

        string Schema { get; }
        string Name { get; }
    }
}