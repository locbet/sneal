using System.Collections.Generic;

namespace Sneal.SqlMigration
{
    public interface IColumn
    {
        IList<IForeignKey> ForeignKeys { get; }
        IList<IIndex> Indexes { get; }

        ITable Table { get; }
        SqlDataType DataType { get; }
        bool IsNullable { get; }
        string Default { get; }

        string Schema { get; }
        string Name { get; }
    }
}