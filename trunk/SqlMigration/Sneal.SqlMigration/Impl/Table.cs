using System;
using System.Collections.Generic;
using System.Text;

namespace Sneal.SqlMigration.Impl
{
    public class Table : ITable
    {
        private IList<IColumn> columns;
        private string schema;
        private string name;

        public IList<IColumn> Columns
        {
            get { return columns; }
        }

        public string Schema
        {
            get { return schema; }
        }

        public string Name
        {
            get { return name; }
        }
    }
}
