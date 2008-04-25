using System.IO;
using NUnit.Framework;
using Rhino.Mocks;
using Sneal.SqlMigration.Impl;
using Sneal.SqlMigration.IO;

namespace Sneal.SqlMigration.Tests
{
    [TestFixture]
    public class ScriptFileFixture
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            fileSystem = mocks.DynamicMock<IFileSystem>();
        }

        #endregion

        private MockRepository mocks;
        private IFileSystem fileSystem;


        [Test]
        public void ShouldReturnFileAsStream()
        {
            MemoryStream stream = new MemoryStream();
            string path = @"c:\temp\data\script.sql";
            Expect.Call(fileSystem.OpenFileStream(path, FileMode.Open)).Return(stream);

            mocks.ReplayAll();

            ScriptFile script = new ScriptFile(path);
            script.FileSystem = fileSystem;

            Assert.AreEqual(stream, script.GetContentStream());

            mocks.VerifyAll();
        }

        [Test]
        public void ShouldReturnFileIsSql()
        {
            ScriptFile script = new ScriptFile(@"c:\temp\whatever.sql");
            Assert.IsTrue(script.IsSql);
            Assert.IsFalse(script.IsXml);
        }

        [Test]
        public void ShouldReturnFileIsXml()
        {
            ScriptFile script = new ScriptFile(@"c:\temp\whatever.xml");
            Assert.IsTrue(script.IsXml);
            Assert.IsFalse(script.IsSql);
        }
    }
}