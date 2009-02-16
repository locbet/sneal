using Sneal.JsUnitUtils.Browsers;
using Sneal.JsUnitUtils.TestFileReaders;

namespace Sneal.JsUnitUtils
{
    /// <summary>
    /// Encompasses the configuration to bootstrap the JsUnit runner.
    /// </summary>
    public class Configuration
    {
        public With Browser = With.InternetExplorer;
        public string WebRootDirectory;
        public string TestFixtureRunnerPath;
        public int FixtureTimeoutInSeconds;
        private readonly AssignedTestFileReader testFileReader = new AssignedTestFileReader();

        /// <summary>
        /// Queues a JsUnit local file path to for testing.
        /// </summary>
        /// <param name="localPathToJsUnitTest">The full path to the HTML test file.</param>
        public void AddTestFixtureFile(string localPathToJsUnitTest)
        {
            testFileReader.AddTestFixtureFile(localPathToJsUnitTest);
        }

        public ITestFileReader TestFileReader
        {
            get { return testFileReader; }
        }
    }
}
