using System;
using MyMeta;

namespace Sneal.SqlMigration.Tests.TestHelpers
{
    internal class ProcedureStub : IProcedure
    {
        internal DatabaseStub database;
        internal string name;
        internal string schema;
        internal short type;
        internal string procedureText;
        internal string description;

        internal ProcedureStub(DatabaseStub db, string name)
        {
            database = db;
            this.name = name;
        }

        internal ProcedureStub(DatabaseStub db, string name, string text)
        {
            database = db;
            this.name = name;
            procedureText = text;
        }

        public object DatabaseSpecificMetaData(string key)
        {
            throw new NotImplementedException();
        }

        public IParameters Parameters
        {
            get { throw new NotImplementedException(); }
        }

        public IResultColumns ResultColumns
        {
            get { throw new NotImplementedException(); }
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
            get { return database; }
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

        public short Type
        {
            get { return type; }
        }

        public string ProcedureText
        {
            get { return procedureText; }
        }

        public string Description
        {
            get { return description; }
        }

        public DateTime DateCreated
        {
            get { throw new NotImplementedException(); }
        }

        public DateTime DateModified
        {
            get { throw new NotImplementedException(); }
        }
    }
}
