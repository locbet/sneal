using System;
using System.Collections.Generic;
using System.Text;

namespace Sneal.SqlMigration.Impl
{
    [Serializable]
    public class Database : IDatabase
    {
        private IList<ITable> tables;
        private IList<IView> views;
        private IList<ISproc> sprocs;
        private string name;
        private bool isLatestVersion;

        public virtual IList<ITable> Tables
        {
            get { return tables; }
            set { tables = value; }
        }

        public virtual IList<IView> Views
        {
            get { return views; }
            set { views = value; }
        }

        public virtual IList<ISproc> Sprocs
        {
            get { return sprocs; }
            set { sprocs = value; }
        }

        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }

        public virtual bool IsLatestVersion
        {
            get { return isLatestVersion; }
            set { isLatestVersion = value; }
        }
    }
}
