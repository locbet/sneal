using System;
using System.Collections.Generic;

namespace Sneal.SqlMigration.Impl
{
    [Serializable]
    public class Column : IColumn, IEquatable<Column>
    {
        private string _default;
        private SqlDataType dataType;
        private IForeignKey foreignKey;
        private IList<IIndex> indexes;
        private bool isNullable;
        private string name;
        private string schema;
        private ITable table;

        #region IColumn Members

        public virtual IForeignKey ForeignKey
        {
            get { return foreignKey; }
            set { foreignKey = value; }
        }

        public virtual IList<IIndex> Indexes
        {
            get { return indexes; }
            set { indexes = value; }
        }

        public virtual ITable Table
        {
            get { return table; }
            set { table = value; }
        }

        public virtual SqlDataType DataType
        {
            get { return dataType; }
            set { dataType = value; }
        }

        public virtual bool IsNullable
        {
            get { return isNullable; }
            set { isNullable = value; }
        }

        public virtual string Default
        {
            get { return _default; }
            set { _default = value; }
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

        public bool Equals(Column column)
        {
            if (column == null) return false;
            if (!Equals(_default, column._default)) return false;
            if (!Equals(dataType, column.dataType)) return false;
            if (!Equals(isNullable, column.isNullable)) return false;
            if (!Equals(name, column.name)) return false;
            if (!Equals(schema, column.schema)) return false;
            if (!Equals(table, column.table)) return false;
            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as Column);
        }

        public override int GetHashCode()
        {
            int result = _default != null ? _default.GetHashCode() : 0;
            result = 29*result + dataType.GetHashCode();
            result = 29*result + isNullable.GetHashCode();
            result = 29*result + name.GetHashCode();
            result = 29*result + schema.GetHashCode();
            result = 29*result + table.GetHashCode();
            return result;
        }
    }
}