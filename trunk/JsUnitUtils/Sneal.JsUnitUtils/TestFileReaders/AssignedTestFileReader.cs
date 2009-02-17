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

namespace Sneal.JsUnitUtils.TestFileReaders
{
    public class AssignedTestFileReader : ITestFileReader
    {
        private readonly List<string> testFixturePaths = new List<string>();
        private int index;

        public void AddTestFixtureFile(string localPathToJsUnitTest)
        {
            testFixturePaths.Add(localPathToJsUnitTest);
        }

        public IEnumerator<string> GetEnumerator()
        {
            return testFixturePaths.GetEnumerator();
        }

        public string GetNextTestFile()
        {
            int num;
            if ((testFixturePaths.Count == 0) || (index >= testFixturePaths.Count))
            {
                return null;
            }
            index = (num = index) + 1;
            return testFixturePaths[num];            
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
