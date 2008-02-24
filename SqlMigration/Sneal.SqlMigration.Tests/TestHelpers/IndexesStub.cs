using System;
using System.Collections;
using System.Collections.Generic;
using MyMeta;

namespace Sneal.SqlMigration.Tests.TestHelpers
{
    internal class IndexesStub : List<IIndex>, IIndexes
    {
        #region IIndexes Members

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

        public IIndex this[object index]
        {
            get
            {
                if (index is int)
                    return base[(int) index];

                string indexName = index as string;
                return Find(delegate(IIndex curIndex) { return curIndex.Name == indexName; });
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