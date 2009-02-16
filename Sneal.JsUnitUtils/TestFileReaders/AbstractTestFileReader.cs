#region license
// Copyright 2008 Shawn Neal (sneal@sneal.net)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System.Collections;
using System.Collections.Generic;
using Sneal.Preconditions.Aop;

namespace Sneal.JsUnitUtils.TestFileReaders
{
    public abstract class AbstractTestFileReader : ITestFileReader
    {
        private readonly ITestFileReader adaptee;

        public AbstractTestFileReader([NotNull]ITestFileReader adaptee)
        {
            this.adaptee = adaptee;
        }

        public string GetNextTestFile()
        {
            string nextTestFile = adaptee.GetNextTestFile();
            if (nextTestFile == null)
            {
                return null;
            }
            if (CurrentFileShouldPassThrough(nextTestFile))
            {
                return nextTestFile;
            }
            return GetNextTestFile();
        }

        public IEnumerator<string> GetEnumerator()
        {
            string curFile;
            while ((curFile = GetNextTestFile()) != null)
            {
                yield return curFile;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected abstract bool CurrentFileShouldPassThrough(string currentFile);
    }
}