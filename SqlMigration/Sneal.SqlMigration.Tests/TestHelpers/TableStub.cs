using System;
using MyMeta;

namespace Sneal.SqlMigration.Tests.TestHelpers
{
    internal class TableStub : ITable
    {
        internal ColumnsStub columns = new ColumnsStub();
        internal IDatabase db;
        internal ForeignKeysStub fks = new ForeignKeysStub();
        internal IndexesStub indexes = new IndexesStub();
        internal string name;
        internal ColumnsStub pks = new ColumnsStub();
        internal string schema;

        public TableStub(IDatabase db, string name)
        {
            this.db = db;
            this.name = name;
            schema = "dbo";
        }

        #region ITable Members

        public object DatabaseSpecificMetaData(string key)
        {
            throw new NotImplementedException();
        }

        public IColumns Columns
        {
            get { return columns; }
        }

        public IForeignKeys ForeignKeys
        {
            get { return fks; }
        }

        public IIndexes Indexes
        {
            get { return indexes; }
        }

        public IColumns PrimaryKeys
        {
            get { return pks; }
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

        public IDatabase Database
        {
            get { return db; }
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

        public string Type
        {
            get { throw new NotImplementedException(); }
        }

        public Guid Guid
        {
            get { throw new NotImplementedException(); }
        }

        public string Description
        {
            get { throw new NotImplementedException(); }
        }

        public int PropID
        {
            get { throw new NotImplementedException(); }
        }

        public DateTime DateCreated
        {
            get { throw new NotImplementedException(); }
        }

        public DateTime DateModified
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        public ColumnStub AddStubbedColumn(string columnName, string dataTypeNameComplete)
        {
            ColumnStub newCol = new ColumnStub(this, columnName, dataTypeNameComplete);
            Columns.Add(newCol);
            return newCol;
        }

        public IndexStub AddStubbedIndex(ColumnStub parentColumn, string indexName)
        {
            IndexStub idx = new IndexStub(parentColumn.Table, parentColumn, indexName);
            Indexes.Add(idx);
            return idx;
        }
    }
}