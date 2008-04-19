using System;
using MyMeta;

namespace Sneal.SqlMigration.Tests.TestHelpers
{
    internal class IndexStub : IIndex
    {
        internal bool clustered = false;
        internal IColumns columns = new ColumnsStub();
        internal string name;
        internal string schema = "dbo";
        internal ITable table;
        internal bool unique = true;
        internal string collation = "ASC";
        internal int fillFactor;

        public IndexStub(ITable table, IColumn column, string name)
        {
            this.table = table;
            this.columns.Add(column);
            this.name = name;
        }

        #region IIndex Members

        public object DatabaseSpecificMetaData(string key)
        {
            throw new NotImplementedException();
        }

        public ITable Table
        {
            get { return table; }
        }

        public IColumns Columns
        {
            get { return columns; }
        }

        public IPropertyCollection Properties
        {
            get { throw new NotImplementedException(); }
        }

        public IPropertyCollection GlobalProperties
        {
            get { throw new NotImplementedException(); }
        }

        public IPropertyCollection AllProperties
        {
            get { throw new NotImplementedException(); }
        }

        public string UserDataXPath
        {
            get { throw new NotImplementedException(); }
        }

        public string Alias
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public string Name
        {
            get { return name; }
        }

        public string Schema
        {
            get { return schema; }
        }

        public bool Unique
        {
            get { return unique; }
        }

        public bool Clustered
        {
            get { return clustered; }
        }

        public string Type
        {
            get { throw new NotImplementedException(); }
        }

        public int FillFactor
        {
            get { return fillFactor; }
        }

        public int InitialSize
        {
            get { throw new NotImplementedException(); }
        }

        public bool SortBookmarks
        {
            get { throw new NotImplementedException(); }
        }

        public bool AutoUpdate
        {
            get { throw new NotImplementedException(); }
        }

        public string NullCollation
        {
            get { throw new NotImplementedException(); }
        }

        public string Collation
        {
            get { return collation; }
        }

        public decimal Cardinality
        {
            get { throw new NotImplementedException(); }
        }

        public int Pages
        {
            get { throw new NotImplementedException(); }
        }

        public string FilterCondition
        {
            get { throw new NotImplementedException(); }
        }

        public bool Integrated
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}