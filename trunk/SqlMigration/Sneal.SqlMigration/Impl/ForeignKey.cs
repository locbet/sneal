using System;

namespace Sneal.SqlMigration.Impl
{
    [Serializable]
    public class ForeignKey : IForeignKey
    {
        private IColumn foreignKeyColumn;
        private string name;
        private IColumn primaryKeyColumn;
        private string schema;

        #region IForeignKey Members

        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }

        public virtual string Schema
        {
            get { return schema; }
            set { schema = value; }
        }

        public virtual IColumn PrimaryKeyColumn
        {
            get { return primaryKeyColumn; }
            set { primaryKeyColumn = value; }
        }

        public virtual IColumn ForeignKeyColumn
        {
            get { return foreignKeyColumn; }
            set { foreignKeyColumn = value; }
        }

        #endregion
    }
}