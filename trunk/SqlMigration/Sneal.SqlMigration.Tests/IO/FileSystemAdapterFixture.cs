using System.IO;
using NUnit.Framework;
using Sneal.SqlMigration.IO;

namespace Sneal.SqlMigration.Tests.IO
{
    [TestFixture]
    public class FileSystemAdapterFixture
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            filePath = Path.GetTempFileName();
        }

        [TearDown]
        public void Dispose()
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        #endregion

        private FileSystemAdapter fileSystem = new FileSystemAdapter();
        private string filePath;

        [Test]
        public void ShouldReturnDirExists()
        {
            string dir = Path.Combine(Path.GetTempPath(), "FileSystemAdapterFixture");
            if (Directory.Exists(dir))
                Directory.Delete(dir);

            Assert.IsFalse(fileSystem.Exists(dir));
            Directory.CreateDirectory(dir);
            Assert.IsTrue(fileSystem.Exists(dir));

            Directory.Delete(dir);
        }

        [Test]
        public void ShouldReturnFileExists()
        {
            if (File.Exists(filePath))
                File.Delete(filePath);

            Assert.IsFalse(fileSystem.Exists(filePath));
            using (File.CreateText(filePath)) {}
            Assert.IsTrue(fileSystem.Exists(filePath));
        }

        [Test]
        public void ShouldReadAllFromFile()
        {
            string contents = "Hello World\r\nBye!";

            using (TextWriter writer = new StreamWriter(filePath))
            {
                writer.Write(contents);
            }

            Assert.AreEqual(contents, fileSystem.ReadToEnd(filePath), "contents differ");
        }
    }
}