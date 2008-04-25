using System.IO;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Sneal.SqlMigration.IO;
using Sneal.SqlMigration.Migrators;

namespace Sneal.SqlMigration.Tests.Migrators
{
    [TestFixture]
    public class SqlServerBulkXmlExecutorFixture
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            scriptFile = mocks.DynamicMock<IScriptFile>();
            fileSystem = mocks.DynamicMock<IFileSystem>();

            executor = new SqlServerBulkXmlExecutor();
            executor.FileSystem = fileSystem;
        }

        #endregion

        private SqlServerBulkXmlExecutor executor;
        private MockRepository mocks;
        private IScriptFile scriptFile;
        private IFileSystem fileSystem;

        [Test]
        public void ShouldGetTableName()
        {
            SetupResult.For(fileSystem.Exists(null)).Return(true).IgnoreArguments();

            string xmlDoc = "<ROOT><Customer><CustomerID>1</CustomerID></Customer></ROOT>";
            MemoryStream content = new MemoryStream(UTF8Encoding.UTF8.GetBytes(xmlDoc));

            SetupResult.For(scriptFile.IsXml).Return(true);
            SetupResult.For(scriptFile.GetContentStream()).Return(content);

            mocks.ReplayAll();

            string tableName = executor.GetTableNameFromXmlElement(scriptFile);
            Assert.AreEqual("Customer", tableName);
        }
    }
}