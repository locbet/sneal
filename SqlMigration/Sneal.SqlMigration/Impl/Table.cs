using System;
using System.Collections.Generic;

namespace Sneal.SqlMigration.Impl
{
    [Serializable]
    public class Table : ITable
    {
        private IList<IColumn> columns;
        private string name;
        private string schema;

        #region ITable Members

        public virtual IList<IColumn> Columns
        {
            get { return columns; }
            set { columns = value; }
        }

        public virtual string Schema
        {
            get { return schema; }
            set { schema = value; }
        }

        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }

        #endregion
    }
}