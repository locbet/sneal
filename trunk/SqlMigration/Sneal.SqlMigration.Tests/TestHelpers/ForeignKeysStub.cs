using System;
using System.Collections;
using System.Collections.Generic;
using MyMeta;

namespace Sneal.SqlMigration.Tests.TestHelpers
{
    internal class ForeignKeysStub : List<IForeignKey>, IForeignKeys
    {
        #region IForeignKeys Members

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

        public IForeignKey this[object index]
        {
            get
            {
                if (index is int)
                    return base[(int) index];

                string fkName = index as string;
                return Find(delegate(IForeignKey curFK) { return curFK.Name == fkName; });
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