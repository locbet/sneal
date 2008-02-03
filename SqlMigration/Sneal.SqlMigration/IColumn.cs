using System;
using System.Collections.Generic;
using System.Text;

namespace Sneal.SqlMigration
{
    public interface IColumn
    {
        IList<IForeignKey> ForeignKeys { get; }
        IList<IIndex> Indexes { get; }

        ITable Table { get; }
        SqlDataType DataType { get; }
        bool IsNullable { get; }

        string Schema { get; }
        string Name { get; }
    }
}
