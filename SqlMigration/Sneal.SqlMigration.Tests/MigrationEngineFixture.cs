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
        public void ShouldThrowIfEitherDBIsNull()
        {
            engine.ScriptDifferences(null, null);
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

        [Test]
        public void ShouldScriptNothing()
        {
            mocks.ReplayAll();

            SqlScript script = engine.ScriptDifferences(srcDB, targetDB);
            Assert.IsNotNull(script);
            Assert.AreEqual(0, script.Length);
        }

        [Test]
        public void ShouldPrintBeginAndEndScriptingMessage()
        {
            msgManager.OnScriptMessage("Starting database differencing.");
            msgManager.OnScriptMessage("Finished database differencing.");
            
            mocks.ReplayAll();

            engine.ScriptDifferences(srcDB, targetDB);
        }

        [Test]
        public void ShouldScriptNewTable()
        {
            TableStub srcTable = srcDB.AddStubbedTable("Customer");
            srcTable.AddStubbedColumn("CustomerID", "INT");

            Expect.Call(scriptBuilder.Create(srcTable)).Return(new SqlScript("--CREATE TABLE Customer"));

            mocks.ReplayAll();

            SqlScript script = engine.ScriptDifferences(srcDB, targetDB);
            Assert.IsNotNull(script);
            Assert.That(script.Length, Is.GreaterThan(0));
        }

        [Test]
        public void ShouldScriptDropTable()
        {
            TableStub targetTable = targetDB.AddStubbedTable("Customer");
            targetTable.AddStubbedColumn("CustomerID", "INT");

            Expect.Call(scriptBuilder.Drop(targetTable)).Return(new SqlScript("--DROP TABLE Customer"));

            mocks.ReplayAll();

            SqlScript script = engine.ScriptDifferences(srcDB, targetDB);
            Assert.IsNotNull(script);
            Assert.That(script.Length, Is.GreaterThan(0));
        }

        [Test]
        public void ShouldScriptNewColumn()
        {
            TableStub srcTable = srcDB.AddStubbedTable("Customer");
            ColumnStub srcCol = srcTable.AddStubbedColumn("CustomerID", "INT");

            targetDB.AddStubbedTable("Customer");

            Expect.Call(scriptBuilder.Create(srcCol)).Return(new SqlScript("--ADD NEW COLUMN CustomerID"));

            mocks.ReplayAll();

            SqlScript script = engine.ScriptDifferences(srcDB, targetDB);
            Assert.IsNotNull(script);
            Assert.That(script.Length, Is.GreaterThan(0));
        }

        [Test]
        public void ShouldScriptDropColumn()
        {
            srcDB.AddStubbedTable("Customer");

            TableStub targetTable = targetDB.AddStubbedTable("Customer");
            ColumnStub targetCol = targetTable.AddStubbedColumn("CustomerID", "INT");

            Expect.Call(scriptBuilder.Drop(targetCol)).Return(new SqlScript("--DROP COLUMN CustomerID"));

            mocks.ReplayAll();

            SqlScript script = engine.ScriptDifferences(srcDB, targetDB);
            Assert.IsNotNull(script);
            Assert.That(script.Length, Is.GreaterThan(0));
        }

        [Test]
        public void ShouldScriptNewIndex()
        {
            TableStub srcTable = srcDB.AddStubbedTable("Customer");
            ColumnStub srcCol = srcTable.AddStubbedColumn("Name", "NVARCHAR(50)");
            IndexStub srcIndex = srcTable.AddStubbedIndex(srcCol, "IX_Name");

            TableStub targetTable = targetDB.AddStubbedTable("Customer");
            targetTable.AddStubbedColumn("Name", "NVARCHAR(50)");

            Expect.Call(scriptBuilder.Create(srcIndex)).Return(new SqlScript("--ADD NEW INDEX IX_Name"));

            mocks.ReplayAll();

            SqlScript script = engine.ScriptDifferences(srcDB, targetDB);
            Assert.IsNotNull(script);
            Assert.That(script.Length, Is.GreaterThan(0));
        }

        [Test]
        public void ShouldScriptDropIndex()
        {
            TableStub srcTable = srcDB.AddStubbedTable("Customer");
            ColumnStub srcCol = srcTable.AddStubbedColumn("Name", "NVARCHAR(50)");

            TableStub targetTable = targetDB.AddStubbedTable("Customer");
            ColumnStub targetCol = targetTable.AddStubbedColumn("Name", "NVARCHAR(50)");
            IndexStub targetIdx = targetTable.AddStubbedIndex(targetCol, "IX_Name");

            Expect.Call(scriptBuilder.Drop(targetIdx)).Return(new SqlScript("--DROP INDEX IX_Name"));

            mocks.ReplayAll();

            SqlScript script = engine.ScriptDifferences(srcDB, targetDB);
            Assert.IsNotNull(script);
            Assert.That(script.Length, Is.GreaterThan(0));
        }

        [Test]
        public void ShouldScriptNewFK()
        {
            TableStub srcTable = srcDB.AddStubbedTable("Customer");
            ColumnStub srcCol = srcTable.AddStubbedColumn("BillingID", "INT");
            ForeignKeyStub fk = srcCol.AddForeignKeyStub("FK_Customer_BillingID");

            TableStub srcPkTable = srcDB.AddStubbedTable("Billing");
            ColumnStub srcPkCol = srcPkTable.AddStubbedColumn("BillingID", "INT");

            fk.primaryColumns.Add(srcPkCol);
            fk.primaryTable = srcPkTable;

            TableStub targetTable = targetDB.AddStubbedTable("Customer");
            targetTable.AddStubbedColumn("BillingID", "INT");

            TableStub targetPkTable = targetDB.AddStubbedTable("Billing");
            targetPkTable.AddStubbedColumn("BillingID", "INT");

            Expect.Call(scriptBuilder.Create(fk)).Return(new SqlScript("--ADD NEW FK FK_BillingID"));

            mocks.ReplayAll();

            SqlScript script = engine.ScriptDifferences(srcDB, targetDB);
            Assert.IsNotNull(script);
            Assert.That(script.Length, Is.GreaterThan(0));
        }

        [Test]
        public void ShouldScriptDropFK()
        {
            TableStub srcTable = srcDB.AddStubbedTable("Customer");
            srcTable.AddStubbedColumn("BillingID", "INT");
            
            TableStub srcPkTable = srcDB.AddStubbedTable("Billing");
            srcPkTable.AddStubbedColumn("BillingID", "INT");

            TableStub targetTable = targetDB.AddStubbedTable("Customer");
            ColumnStub targetCol = targetTable.AddStubbedColumn("BillingID", "INT");
            ForeignKeyStub fk = targetCol.AddForeignKeyStub("FK_Customer_BillingID");

            TableStub targetPkTable = targetDB.AddStubbedTable("Billing");
            ColumnStub targetPkCol = targetPkTable.AddStubbedColumn("BillingID", "INT");

            fk.primaryColumns.Add(targetPkCol);
            fk.primaryTable = targetPkTable;

            Expect.Call(scriptBuilder.Drop(fk)).Return(new SqlScript("--DROP FK FK_BillingID"));

            mocks.ReplayAll();

            SqlScript script = engine.ScriptDifferences(srcDB, targetDB);
            Assert.IsNotNull(script);
            Assert.That(script.Length, Is.GreaterThan(0));
        }

        [Test]
        [Ignore("Not implmented - TDD")]
        public void ShouldScriptCreateSproc()
        {
            ProcedureStub sproc = srcDB.AddStubbedProcedure("SP_CREATEUSER");

            Expect.Call(scriptBuilder.Create(sproc)).Return(new SqlScript("--CREATE PROC US_123"));

            mocks.ReplayAll();

            SqlScript script = engine.ScriptDifferences(srcDB, targetDB);
            Assert.IsNotNull(script);
            Assert.That(script.Length, Is.GreaterThan(0));            
        }

        [Test]
        [Ignore("Not implmented - TDD")]
        public void ShouldScriptAlterSproc()
        {
            ProcedureStub srcProc = srcDB.AddStubbedProcedure("SP_CREATEUSER", "SRC TEXT");
            targetDB.AddStubbedProcedure("SP_CREATEUSER", "TARGET TEXT");

            Expect.Call(scriptBuilder.Alter(srcProc)).Return(new SqlScript("--ALTER PROC US_123"));

            mocks.ReplayAll();

            SqlScript script = engine.ScriptDifferences(srcDB, targetDB);
            Assert.IsNotNull(script);
            Assert.That(script.Length, Is.GreaterThan(0)); 
        }

        [Test]
        [Ignore("Not implmented - TDD")]
        public void ShouldScriptDropSproc()
        {
            ProcedureStub targetSproc = targetDB.AddStubbedProcedure("SP_CREATEUSER", "TARGET TEXT");

            Expect.Call(scriptBuilder.Drop(targetSproc)).Return(new SqlScript("--DROP PROC US_123"));

            mocks.ReplayAll();

            SqlScript script = engine.ScriptDifferences(srcDB, targetDB);
            Assert.IsNotNull(script);
            Assert.That(script.Length, Is.GreaterThan(0)); 
        }
    }
}