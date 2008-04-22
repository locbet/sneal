using System;
using MyMeta;

namespace Sneal.SqlMigration.Tests.TestHelpers
{
    internal class ColumnStub : IColumn
    {
        internal string colDefault;
        internal string dataTypeNameComplete;
        internal string domainSchema;
        internal ForeignKeysStub foreignKeys = new ForeignKeysStub();
        internal bool isAutoKey;
        internal bool isInPrimaryKey;
        internal bool isNullable;
        internal string name;
        internal ITable table;
        internal int dataType;
        internal bool hasDefault;
        internal bool isComputed;

        public ColumnStub(ITable table, string name, string dataTypeNameComplete)
        {
            this.table = table;
            this.name = name;
            this.dataTypeNameComplete = dataTypeNameComplete;
        }

        #region IColumn Members

        public object DatabaseSpecificMetaData(string key)
        {
            throw new NotImplementedException();
        }

        public ITable Table
        {
            get { return table; }
        }

        public IView View
        {
            get { throw new NotImplementedException(); }
        }

        public IDomain Domain
        {
            get { throw new NotImplementedException(); }
        }

        public IForeignKeys ForeignKeys
        {
            get { return foreignKeys; }
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

        public int DataType
        {
            get { return dataType; }
            set { dataType = value; }
        }

        public string DataTypeName
        {
            get { throw new NotImplementedException(); }
        }

        public string DataTypeNameComplete
        {
            get { return dataTypeNameComplete; }
        }

        public string LanguageType
        {
            get { throw new NotImplementedException(); }
        }

        public string DbTargetType
        {
            get { throw new NotImplementedException(); }
        }

        public Guid Guid
        {
            get { throw new NotImplementedException(); }
        }

        public int PropID
        {
            get { throw new NotImplementedException(); }
        }

        public int Ordinal
        {
            get { throw new NotImplementedException(); }
        }

        public bool HasDefault
        {
            get { return hasDefault; }
        }

        public string Default
        {
            get { return colDefault; }
        }

        public int Flags
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsNullable
        {
            get { return isNullable; }
        }

        public Guid TypeGuid
        {
            get { throw new NotImplementedException(); }
        }

        public int CharacterMaxLength
        {
            get { throw new NotImplementedException(); }
        }

        public int CharacterOctetLength
        {
            get { throw new NotImplementedException(); }
        }

        public int NumericPrecision
        {
            get { throw new NotImplementedException(); }
        }

        public int NumericScale
        {
            get { throw new NotImplementedException(); }
        }

        public int DateTimePrecision
        {
            get { throw new NotImplementedException(); }
        }

        public string CharacterSetCatalog
        {
            get { throw new NotImplementedException(); }
        }

        public string CharacterSetSchema
        {
            get { throw new NotImplementedException(); }
        }

        public string CharacterSetName
        {
            get { throw new NotImplementedException(); }
        }

        public string DomainCatalog
        {
            get { throw new NotImplementedException(); }
        }

        public string DomainSchema
        {
            get { return domainSchema; }
        }

        public string DomainName
        {
            get { throw new NotImplementedException(); }
        }

        public string Description
        {
            get { throw new NotImplementedException(); }
        }

        public int LCID
        {
            get { throw new NotImplementedException(); }
        }

        public int CompFlags
        {
            get { throw new NotImplementedException(); }
        }

        public int SortID
        {
            get { throw new NotImplementedException(); }
        }

        public byte[] TDSCollation
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsComputed
        {
            get { return isComputed; }
        }

        public bool IsInPrimaryKey
        {
            get { return isInPrimaryKey; }
        }

        public bool IsAutoKey
        {
            get { return isAutoKey; }
        }

        public bool IsInForeignKey
        {
            get { return foreignKeys.Count > 0; }
        }

        public int AutoKeySeed
        {
            get { throw new NotImplementedException(); }
        }

        public int AutoKeyIncrement
        {
            get { throw new NotImplementedException(); }
        }

        public bool HasDomain
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        /// <summary>
        /// Adds a foriegn key that is NOT associated with a primary table/column.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ForeignKeyStub AddForeignKeyStub(string name)
        {
            ForeignKeyStub fk = new ForeignKeyStub(table, name);
            fk.foreignColumns.Add(this);
            fk.foreignTable = this.table;

            foreignKeys.Add(fk);
            table.ForeignKeys.Add(fk);
            return fk;
        }
    }
}