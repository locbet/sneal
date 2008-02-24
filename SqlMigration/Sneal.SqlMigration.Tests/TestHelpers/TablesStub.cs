using System;
using System.Collections;
using System.Collections.Generic;
using MyMeta;

namespace Sneal.SqlMigration.Tests.TestHelpers
{
    internal class TablesStub : List<ITable>, ITables
    {
        #region ITables Members

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

        public ITable this[object index]
        {
            get
            {
                if (index is int)
                    return base[(int) index];

                string indexName = index as string;
                return Find(delegate(ITable curIndex) { return curIndex.Name == indexName; });
            }
        }

        public bool IsSynchronized
        {
            get { throw new NotImplementedException(); }
        }

        public object SyncRoot
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}