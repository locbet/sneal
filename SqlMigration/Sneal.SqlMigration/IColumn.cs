using System.Collections.Generic;

namespace Sneal.SqlMigration
{
    public interface IColumn
    {
        IList<IIndex> Indexes { get; }
        IForeignKey ForeignKey { get; }
        ITable Table { get; }

        SqlDataType DataType { get; }
        bool IsNullable { get; }
        string Default { get; }

        string Schema { get; }
        string Name { get; }
    }
}