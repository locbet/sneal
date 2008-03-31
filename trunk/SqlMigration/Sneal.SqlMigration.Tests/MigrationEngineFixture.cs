using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using Sneal.SqlMigration;
using Sneal.SqlMigration.Impl;
using Sneal.SqlMigration.Tests.TestHelpers;

namespace Sneal.SqlMigration.Tests
{
    [TestFixture]
    public class MigrationEngineFixture
    {
        private MigrationEngine engine;
        private MockRepository mocks;
        private IScriptBuilder scriptBuilder;
        private IDatabaseComparer dbComparer;
        private IScriptMessageManager msgManager;

        private DatabaseStub srcDB;
        private DatabaseStub targetDB;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            scriptBuilder = mocks.CreateMock<IScriptBuilder>();
            msgManager = mocks.DynamicMock<IScriptMessageManager>();

            dbComparer = new DatabaseComparer();

            engine = new MigrationEngine(scriptBuilder, dbComparer);
            engine.MessageManager = msgManager;

            srcDB = new DatabaseStub("SRC_DB");
            targetDB = new DatabaseStub("TARGET_DB");
        }

        [TearDown]
        public void TearDown()
        {
            mocks.ReplayAll();  // ensure replay was called
            mocks.VerifyAll();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowIfMessageManagerIsNull()
        {
            engine.MessageManager = null;
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowIfScriptBuilderIsNull()
        {
            new MigrationEngine(null, dbComparer);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowIfDBComparerIsNull()
        {
            new MigrationEngine(scriptBuilder, null);
        }
    }
}