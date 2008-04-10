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
        private IConnectionSettings connSettings;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            scriptBuilder = mocks.CreateMock<IScriptBuilder>();
            msgManager = mocks.DynamicMock<IScriptMessageManager>();
            connSettings = mocks.Stub<IConnectionSettings>();

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
        public void ShouldThrowIfMessageManagerIsNull_MessageManager()
        {
            engine.MessageManager = null;
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowIfScriptBuilderIsNull_Ctor()
        {
            new MigrationEngine(null, dbComparer);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowIfDBComparerIsNull_Ctor()
        {
            new MigrationEngine(scriptBuilder, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowIfConnectionSettingsIsNull_Script()
        {
            engine.Script(null, new ScriptingOptions());
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowIfConnectionSettings1IsNull_ScriptDifferences()
        {
            engine.ScriptDifferences(null, connSettings, new ScriptingOptions());
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowIfConnectionSettings2IsNull_ScriptDifferences()
        {
            engine.ScriptDifferences(connSettings, null, new ScriptingOptions());
        }
    }
}