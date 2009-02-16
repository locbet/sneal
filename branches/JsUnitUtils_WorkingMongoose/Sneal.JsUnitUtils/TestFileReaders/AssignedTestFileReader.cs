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
