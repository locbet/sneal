using System;
using ADODB;
using MyMeta;

namespace Sneal.SqlMigration.Tests.TestHelpers
{
    internal class DatabaseStub : IDatabase
    {
        private string name;
        private ITables tables = new TablesStub();
        internal IProcedures procedures = new ProceduresStub();

        public DatabaseStub(string name)
        {
            this.name = name;
        }

        #region IDatabase Members

        public Recordset ExecuteSql(string sql)
        {
            throw new NotImplementedException();
        }

        public object DatabaseSpecificMetaData(string key)
        {
            throw new NotImplementedException();
        }

        public ITables Tables
        {
            get { return tables; }
        }

        public IViews Views
        {
            get { throw new NotImplementedException(); }
        }

        public IProcedures Procedures
        {
            get { return procedures; }
        }

        public dbRoot Root
        {
            get { throw new NotImplementedException(); }
        }

        public IDomains Domains
        {
            get { throw new NotImplementedException(); }
        }

        public IPropertyCollection Properties
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

        public string Description
        {
            get { throw new NotImplementedException(); }
        }

        public string SchemaName
        {
            get { throw new NotImplementedException(); }
        }

        public string SchemaOwner
        {
            get { throw new NotImplementedException(); }
        }

        public string DefaultCharSetCatalog
        {
            get { throw new NotImplementedException(); }
        }

        public string DefaultCharSetSchema
        {
            get { throw new NotImplementedException(); }
        }

        public string DefaultCharSetName
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        public TableStub AddStubbedTable(string tableName)
        {
            TableStub newTable = new TableStub(this, tableName);
            Tables.Add(newTable);
            return newTable;
        }

        public TableStub AddStubbedTable(TableStub table)
        {
            table.db = this;
            Tables.Add(table);
            return table;
        }

        public ProcedureStub AddStubbedProcedure(string procName)
        {
            ProcedureStub stub = new ProcedureStub(this, procName);
            Procedures.Add(stub);
            return stub;
        }

        public ProcedureStub AddStubbedProcedure(string procName, string procText)
        {
            ProcedureStub stub = new ProcedureStub(this, procName, procText);
            Procedures.Add(stub);
            return stub;
        }
    }
}