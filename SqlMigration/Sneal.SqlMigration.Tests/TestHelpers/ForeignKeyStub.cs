using System;
using MyMeta;

namespace Sneal.SqlMigration.Tests.TestHelpers
{
    internal class ForeignKeyStub : IForeignKey
    {
        internal IColumns foreignColumns = new ColumnsStub();
        internal ITable foreignTable;
        internal string name;
        internal IColumns primaryColumns = new ColumnsStub();
        internal ITable primaryTable;

        internal ForeignKeyStub(ITable foreignTable, string name)
        {
            this.foreignTable = foreignTable;
            this.name = name;
        }

        #region IForeignKey Members

        public object DatabaseSpecificMetaData(string key)
        {
            throw new NotImplementedException();
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

        public ITable PrimaryTable
        {
            get { return primaryTable; }
        }

        public ITable ForeignTable
        {
            get { return foreignTable; }
        }

        public IColumns ForeignColumns
        {
            get { return foreignColumns; }
        }

        public IColumns PrimaryColumns
        {
            get { return primaryColumns; }
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

        public string UpdateRule
        {
            get { throw new NotImplementedException(); }
        }

        public string DeleteRule
        {
            get { throw new NotImplementedException(); }
        }

        public string PrimaryKeyName
        {
            get { throw new NotImplementedException(); }
        }

        public string Deferrability
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}