using NUnit.Framework;
using Rhino.Mocks;
using Sneal.SqlMigration.Impl;
using Sneal.SqlMigration.IO;

namespace Sneal.SqlMigration.Tests
{
    [TestFixture]
    public class DefaultTemplateManagerFixture
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            fileSystem = mocks.DynamicMock<IFileSystem>();
        }

        [TearDown]
        public void TearDown()
        {
            mocks.VerifyAll();
        }

        #endregion

        private IFileSystem fileSystem;
        private MockRepository mocks;

        [Test]
        public void ShouldReturnCreateTable()
        {
            Expect.Call(fileSystem.Exists(@"c:\dir\CreateTable.vm")).Return(true);
            mocks.ReplayAll();

            SqlServerTemplateManager mgr = new SqlServerTemplateManager(fileSystem, @"c:\dir");
            Assert.AreEqual(@"CreateTable.vm", mgr.CreateTable);
        }

        [Test]
        public void ShouldReturnDropTable()
        {
            Expect.Call(fileSystem.Exists(@"c:\dir\DropTable.vm")).Return(true);
            mocks.ReplayAll();

            SqlServerTemplateManager mgr = new SqlServerTemplateManager(fileSystem, @"c:\dir");
            Assert.AreEqual(@"DropTable.vm", mgr.DropTable);
        }
    }
}