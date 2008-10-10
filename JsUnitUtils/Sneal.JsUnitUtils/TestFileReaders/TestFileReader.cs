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
using System.IO;

namespace Sneal.JsUnitUtils.TestFileReaders
{
    public class TestFileReader : ITestFileReader
    {
        private readonly string[] files;
        private int index;

        public TestFileReader(string testDirectory)
            : this(Directory.GetFiles(testDirectory, "*", SearchOption.AllDirectories))
        {
        }

        public TestFileReader(string[] files)
        {
            this.files = files;
        }

        #region ITestFileReader Members

        public string GetNextTestFile()
        {
            int num;
            if ((files.Length == 0) || (index >= files.Length))
            {
                return null;
            }
            index = (num = index) + 1;
            return files[num];
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

        #endregion
    }
}