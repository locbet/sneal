using System;
using NUnit.Framework;
using Rhino.Mocks;
using Sneal.SqlMigration.Impl;
using Sneal.SqlMigration.Tests.TestHelpers;

namespace Sneal.SqlMigration.Tests
{
    [TestFixture]
    public class SqlServerScriptBuilderFixture
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            templateDir = AppDomain.CurrentDomain.BaseDirectory + @"\..\..\..\Sneal.SqlMigration\Templates";

            mocks = new MockRepository();
            templateManager = mocks.DynamicMock<ISqlTemplateManager>();

            SetupResult.For(templateManager.TemplateDirectory).Return(templateDir);
            mocks.Replay(templateManager);
            scriptBuilder = new SqlServerScriptBuilder(templateManager);
            mocks.BackToRecord(templateManager);

            db = new DatabaseStub("DB");
            table = new TableStub(db, "Customer");
        }

        #endregion

        private SqlServerScriptBuilder scriptBuilder;
        private ISqlTemplateManager templateManager;
        private MockRepository mocks;
        private TableStub table;
        private DatabaseStub db;
        private string templateDir;

        [Test]
        public void ShouldScriptAddColumn()
        {
            SetupResult.For(templateManager.CreateColumn).Return(@"AddColumn.vm");
            mocks.ReplayAll();

            ColumnStub column = table.AddStubbedColumn("LastName", "NVARCHAR(50)");
            column.isNullable = false;

            SqlScript script = scriptBuilder.Create(column);
            string sql = script.ToScript();

            Console.WriteLine(sql);
            Assert.IsTrue(sql.Contains("IF NOT EXISTS"), "Missing IF NOT EXISTS");
            Assert.IsTrue(sql.Contains("ALTER TABLE [dbo].[Customer]"), "Missing ALTER TABLE");
            Assert.IsTrue(sql.Contains("ADD [LastName] NVARCHAR(50) NOT NULL"), "Missing ADD column");
        }

        [Test]
        public void ShouldScriptAddForeignKey()
        {
            SetupResult.For(templateManager.CreateForeignKey).Return(@"AddForeignKey.vm");
            mocks.ReplayAll();

            TableStub fkTable = db.AddStubbedTable("Customer");
            ColumnStub fkCol = fkTable.AddStubbedColumn("BillingID", "INT");
            ForeignKeyStub fk = fkCol.AddForeignKeyStub("FK_Customer_BillingID");
            
            TableStub pkTable = db.AddStubbedTable("Billing");
            ColumnStub pkCol = pkTable.AddStubbedColumn("BillingID", "INT");

            fk.primaryColumns.Add(pkCol);
            fk.primaryTable = pkTable;

            SqlScript script = scriptBuilder.Create(fk);
            string sql = script.ToScript();

            Console.WriteLine(sql);
            Assert.IsTrue(sql.Contains("IF NOT EXISTS"), "Missing IF NOT EXISTS");
            Assert.IsTrue(sql.Contains("ALTER TABLE [dbo].[Customer]"), "Missing ALTER TABLE");
            Assert.IsTrue(sql.Contains("ADD CONSTRAINT"), "Missing ADD CONSTRAINT");
            Assert.IsTrue(sql.Contains("FK_Customer_BillingID"), "Missing FK_Customer_BillingID");
            Assert.IsTrue(sql.Contains("REFERENCES [dbo].[Billing]"), "Missing REFERENCES Billing");
        }

        [Test]
        public void ShouldScriptAlterColumn()
        {
            SetupResult.For(templateManager.AlterColumn).Return(@"AlterColumn.vm");
            mocks.ReplayAll();

            ColumnStub column = table.AddStubbedColumn("LastName", "NVARCHAR(50)");
            column.isNullable = true;

            SqlScript script = scriptBuilder.Alter(column);
            string sql = script.ToScript();

            Console.WriteLine(sql);
            Assert.IsTrue(sql.Contains("IF EXISTS"), "Missing IF EXISTS");
            Assert.IsTrue(sql.Contains("ALTER TABLE [dbo].[Customer]"), "Missing ALTER TABLE");
            Assert.IsTrue(sql.Contains("ALTER COLUMN [LastName]"), "Missing ALTER COLUMN");
        }

        [Test]
        public void ShouldScriptCreateIndex()
        {
            SetupResult.For(templateManager.CreateIndex).Return(@"CreateIndex.vm");
            mocks.ReplayAll();

            DatabaseStub db = new DatabaseStub("DB");
            TableStub table = new TableStub(db, "Customer");
            ColumnStub column = table.AddStubbedColumn("LastName", "NVARCHAR(50)");
            IndexStub index = table.AddStubbedIndex(column, "IX_LastName");
            index.unique = false;

            SqlScript script = scriptBuilder.Create(index);
            string sql = script.ToScript();

            Console.WriteLine(sql);
            Assert.IsTrue(sql.Contains("IF NOT EXISTS"), "Missing IF NOT EXISTS");
            Assert.IsTrue(sql.Contains("CREATE NONCLUSTERED INDEX [IX_LastName]"), "Missing CREATE NONCLUSTERED INDEX");
            Assert.IsTrue(sql.Contains("[dbo].[Customer]"), "Missing [dbo].[Customer]");
            Assert.IsTrue(sql.Contains("LastName"), "Missing LastName");
        }

        [Test]
        public void ShouldScriptCreateTable()
        {
            SetupResult.For(templateManager.CreateTable).Return(@"CreateTable.vm");
            mocks.ReplayAll();

            table.AddStubbedColumn("Name", "NVARCHAR(50)");
            table.AddStubbedColumn("Dob", "DATETIME");

            SqlScript script = scriptBuilder.Create(table);
            string sql = script.ToScript();

            Console.WriteLine(sql);
            Assert.IsTrue(sql.Contains("IF NOT EXISTS"), "Missing IF NOT EXISTS");
            Assert.IsTrue(sql.Contains("CREATE TABLE [dbo].[Customer]"));
        }

        [Test]
        public void ShouldScriptCreateUniqueIndex()
        {
            SetupResult.For(templateManager.CreateIndex).Return(@"CreateIndex.vm");
            mocks.ReplayAll();

            ColumnStub column = table.AddStubbedColumn("LastName", "NVARCHAR(50)");
            IndexStub index = table.AddStubbedIndex(column, "IX_LastName");
            index.unique = true;

            SqlScript script = scriptBuilder.Create(index);
            string sql = script.ToScript();

            Console.WriteLine(sql);
            Assert.IsTrue(sql.Contains("IF NOT EXISTS"), "Missing IF NOT EXISTS");
            Assert.IsTrue(sql.Contains("CREATE UNIQUE NONCLUSTERED INDEX [IX_LastName]"),
                          "Missing CREATE NONCLUSTERED INDEX");
            Assert.IsTrue(sql.Contains("[dbo].[Customer]"), "Missing [dbo].[Customer]");
            Assert.IsTrue(sql.Contains("LastName"), "Missing LastName");
        }

        [Test]
        public void ShouldScriptDropColumn()
        {
            SetupResult.For(templateManager.DropColumn).Return(@"DropColumn.vm");
            mocks.ReplayAll();

            ColumnStub column = table.AddStubbedColumn("LastName", "NVARCVHAR(50)");

            SqlScript script = scriptBuilder.Drop(column);
            string sql = script.ToScript();

            Console.WriteLine(sql);
            Assert.IsTrue(sql.Contains("IF EXISTS"), "Missing IF EXISTS");
            Assert.IsTrue(sql.Contains("ALTER TABLE [dbo].[Customer]"), "Missing ALTER TABLE");
            Assert.IsTrue(sql.Contains("DROP [LastName]"), "Missing DROP column");
        }

        [Test]
        public void ShouldScriptDropForeignKey()
        {
            SetupResult.For(templateManager.DropForeignKey).Return(@"DropForeignKey.vm");
            mocks.ReplayAll();

            DatabaseStub db = new DatabaseStub("DB");
            TableStub fkTable = db.AddStubbedTable("Customer");
            ColumnStub fkCol = fkTable.AddStubbedColumn("BillingID", "INT");
            ForeignKeyStub fk = fkCol.AddForeignKeyStub("FK_Customer_BillingID");

            TableStub pkTable = db.AddStubbedTable("Billing");
            ColumnStub pkCol = pkTable.AddStubbedColumn("BillingID", "INT");

            fk.primaryColumns.Add(pkCol);
            fk.primaryTable = pkTable;

            SqlScript script = scriptBuilder.Drop(fk);
            string sql = script.ToScript();

            Console.WriteLine(sql);
            Assert.IsTrue(sql.Contains("IF EXISTS"), "Missing IF EXISTS");
            Assert.IsTrue(sql.Contains("ALTER TABLE [dbo].[Customer]"), "Missing ALTER TABLE");
            Assert.IsTrue(sql.Contains("DROP CONSTRAINT"), "Missing DROP CONSTRAINT");
            Assert.IsTrue(sql.Contains("FK_Customer_BillingID"), "Missing FK_Customer_BillingID");
        }

        [Test]
        public void ShouldScriptDropTable()
        {
            SetupResult.For(templateManager.CreateTable).Return(@"DropTable.vm");

            mocks.ReplayAll();

            SqlScript script = scriptBuilder.Create(table);
            string sql = script.ToScript();

            Console.WriteLine(sql);
            Assert.IsTrue(sql.Contains("IF EXISTS"), "Missing IF EXISTS");
            Assert.IsTrue(sql.Contains("DROP TABLE [dbo].[Customer]"));
        }
    }
}