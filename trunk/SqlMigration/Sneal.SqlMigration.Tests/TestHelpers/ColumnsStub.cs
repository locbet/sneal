using System;
using System.Collections;
using System.Collections.Generic;
using MyMeta;

namespace Sneal.SqlMigration.Tests.TestHelpers
{
    internal class ColumnsStub : List<IColumn>, IColumns
    {
        #region IColumns Members

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public new IEnumerator GetEnumerator()
        {
            return base.GetEnumerator();
        }

        public string UserDataXPath
        {
            get { throw new NotImplementedException(); }
        }

        public IColumn this[object index]
        {
            get
            {
                if (index is int)
                    return base[(int) index];

                string colName = index as string;
                return Find(delegate(IColumn curColumn) { return curColumn.Name == colName; });
            }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}