using System;
using System.Collections.Generic;
using System.Text;

namespace Sneal.SqlMigration.Impl
{
    public class Database : IDatabase
    {
        private IList<ITable> tables;
        private IList<IView> views;
        private IList<ISproc> srocs;
        private string name;
        private bool isLatestVersion;

        public IList<ITable> Tables
        {
            get { return tables; }
        }

        public IList<IView> Views
        {
            get { return views; }
        }

        public IList<ISproc> Srocs
        {
            get { return srocs; }
        }

        public string Name
        {
            get { return name; }
        }

        public bool IsLatestVersion
        {
            get { return isLatestVersion; }
            set { isLatestVersion = value; }
        }
    }
}
