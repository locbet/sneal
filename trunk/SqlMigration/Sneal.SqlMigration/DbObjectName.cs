using System;
using MyMeta;
using Sneal.Preconditions;

namespace Sneal.SqlMigration
{
    /// <summary>
    /// This class represents a database objects qualified name: 
    /// [schema].[object].
    /// <para><example>dbo.Customer</example></para>
    /// <para>
    /// This class is essentially a string wrapper that provides some extra
    /// properties for easily getting a DB object's name parts.  A DbObjectName
    /// value can never be null or empty.
    /// </para>
    /// </summary>
    public class DbObjectName : IEquatable<DbObjectName>
    {
        private readonly string name;

        public DbObjectName(string name)
        {
            Throw.If(name, "name").IsEmpty();
            this.name = name;
        }

        public string Schema
        {
            get
            {
                string[] parts = name.Split('.');
                if (parts.Length < 2)
                    return "";

                return parts[parts.Length - 2];
            }
        }

        public string ShortName
        {
            get
            {
                string[] parts = name.Split('.');
                if (parts.Length < 1)
                    return name;

                return parts[parts.Length - 1];
            }
        }

        #region IEquatable<DbObjectName> Members

        public bool Equals(DbObjectName dbObjectName)
        {
            if (dbObjectName == null) return false;
            return Equals(name, dbObjectName.name);
        }

        #endregion

        public static implicit operator string(DbObjectName dbObjectName)
        {
            return dbObjectName.name;
        }

        public static implicit operator DbObjectName(string name)
        {
            return new DbObjectName(name);
        }

        public override string ToString()
        {
            return name;
        }

        public static DbObjectName CreateDbObjectName(ITable table)
        {
            string name = "";
            if (!string.IsNullOrEmpty(table.Schema))
                name = table.Schema + ".";

            name += table.Name;

            return new DbObjectName(name);
        }

        public static DbObjectName CreateDbObjectName(IView view)
        {
            string name = "";
            if (!string.IsNullOrEmpty(view.Schema))
                name = view.Schema + ".";

            name += view.Name;

            return new DbObjectName(name);
        }

        public static DbObjectName CreateDbObjectName(IProcedure sproc)
        {
            string name = "";
            if (!string.IsNullOrEmpty(sproc.Schema))
                name = sproc.Schema + ".";

            name += sproc.Name;

            return new DbObjectName(name);
        }

        public static bool operator !=(DbObjectName dbObjectName1, DbObjectName dbObjectName2)
        {
            return !Equals(dbObjectName1, dbObjectName2);
        }

        public static bool operator ==(DbObjectName dbObjectName1, DbObjectName dbObjectName2)
        {
            return Equals(dbObjectName1, dbObjectName2);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as DbObjectName);
        }

        public override int GetHashCode()
        {
            return name.GetHashCode();
        }
    }
}