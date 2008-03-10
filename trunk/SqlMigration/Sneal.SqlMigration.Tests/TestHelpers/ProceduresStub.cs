using System;
using System.Collections;
using System.Collections.Generic;
using MyMeta;

namespace Sneal.SqlMigration.Tests.TestHelpers
{
    internal class ProceduresStub : List<IProcedure>, IProcedures
    {
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

        public IProcedure this[object proc]
        {
            get
            {
                if (proc is int)
                    return base[(int)proc];

                string procName = proc as string;
                return Find(delegate(IProcedure curIndex) { return curIndex.Name == procName; });
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
    }
}
