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
