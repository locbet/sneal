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
using Microsoft.Build.Framework;

namespace Sneal.JsUnitUtils.MsBuild
{
    public class TaskItemTestReader : ITestFileReader
    {
        private readonly List<ITaskItem> testFileItems;
        private int index;

        public TaskItemTestReader(IEnumerable<ITaskItem> testFileItems)
        {
            this.testFileItems = new List<ITaskItem>(testFileItems);
        }

        public IEnumerator<string> GetEnumerator()
        {
            foreach (ITaskItem item in testFileItems)
            {
                yield return item.ItemSpec;
            }
        }

        public string GetNextTestFile()
        {
            int num;
            if ((testFileItems.Count == 0) || (index >= testFileItems.Count))
            {
                return null;
            }
            index = (num = index) + 1;
            return testFileItems[num].ItemSpec;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
