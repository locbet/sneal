using System;
using MyMeta;
using NUnit.Framework;
using Rhino.Mocks;
using Sneal.SqlMigration.Impl;

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

            table = mocks.Stub<ITable>();
            SetupResult.For(table.Schema).Return("dbo");
            SetupResult.For(table.Name).Return("Customer");
        }

        #endregion

        private SqlServerScriptBuilder scriptBuilder;
        private ISqlTemplateManager templateManager;
        private MockRepository mocks;
        private ITable table;
        private string templateDir;

        private IIndex CreateIndex(bool isUnique)
        {
            IColumn column = mocks.Stub<IColumn>();
            SetupResult.For(column.Name).Return("LastName");
            SetupResult.For(column.IsNullable).Return(false);
            SetupResult.For(column.DataTypeNameComplete).Return("NVARCHAR(50)");
            SetupResult.For(column.Table).Return(table);

            IColumns columns = mocks.Stub<IColumns>();
            SetupResult.For(columns[0]).Return(column);
            SetupResult.For(columns.Count).Return(1);

            IIndex index = mocks.Stub<IIndex>();
            SetupResult.For(index.Columns).Return(columns);
            SetupResult.For(index.Schema).Return("dbo");
            SetupResult.For(index.Name).Return("IX_LastName");
            SetupResult.For(index.Unique).Return(isUnique);
            SetupResult.For(index.Table).Return(table);

            return index;
        }

        [Test]
        public void ShouldScriptAddColumn()
        {
            SetupResult.For(templateManager.CreateColumn).Return(@"AddColumn.vm");

            IColumn column = mocks.Stub<IColumn>();

            SetupResult.For(column.Name).Return("LastName");
            SetupResult.For(column.IsNullable).Return(false);
            SetupResult.For(column.DataTypeNameComplete).Return("NVARCHAR(50)");
            SetupResult.For(column.Table).Return(table);

            mocks.ReplayAll();

            SqlScript script = scriptBuilder.Create(column);
            string sql = script.ToScript();

            Console.WriteLine(sql);
            Assert.IsTrue(sql.Contains("IF NOT EXISTS"), "Missing IF NOT EXISTS");
            Assert.IsTrue(sql.Contains("ALTER TABLE [dbo].[Customer]"), "Missing ALTER TABLE");
            Assert.IsTrue(sql.Contains("ADD [LastName] [NVARCHAR] (50) NOT NULL"), "Missing ADD column");
        }

        [Test]
        public void ShouldScriptAddForeignKey()
        {
            SetupResult.For(templateManager.CreateForeignKey).Return(@"AddForeignKey.vm");

            ITable pkTable = mocks.Stub<ITable>();
            SetupResult.For(pkTable.Name).Return("Billing");
            SetupResult.For(pkTable.Schema).Return("dbo");

            IColumn fkColumn = mocks.Stub<IColumn>();
            SetupResult.For(fkColumn.Name).Return("BillingID");
            SetupResult.For(fkColumn.IsNullable).Return(true);
            SetupResult.For(fkColumn.DataTypeNameComplete).Return("int");
            SetupResult.For(fkColumn.Table).Return(table);

            IColumn pkColumn = mocks.Stub<IColumn>();
            SetupResult.For(pkColumn.Name).Return("BillingID");
            SetupResult.For(pkColumn.IsNullable).Return(true);
            SetupResult.For(pkColumn.DataTypeNameComplete).Return("int");
            SetupResult.For(pkColumn.Table).Return(pkTable);

            IColumns foreignColumns = mocks.Stub<IColumns>();
            IColumns primaryColumns = mocks.Stub<IColumns>();

            SetupResult.For(foreignColumns[0]).Return(fkColumn);
            SetupResult.For(foreignColumns.Count).Return(1);
            SetupResult.For(primaryColumns[0]).Return(pkColumn);
            SetupResult.For(primaryColumns.Count).Return(1);

            IForeignKey fk = mocks.Stub<IForeignKey>();
            SetupResult.For(fk.ForeignColumns).Return(foreignColumns);
            SetupResult.For(fk.PrimaryColumns).Return(primaryColumns);
            SetupResult.For(fk.Name).Return("FK_Customer_BillingID");

            mocks.ReplayAll();

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

            IColumn column = mocks.Stub<IColumn>();
            SetupResult.For(column.DomainSchema).Return("dbo");
            SetupResult.For(column.Name).Return("LastName");
            SetupResult.For(column.DataTypeNameComplete).Return("NVARCHAR(50)");
            SetupResult.For(column.IsNullable).Return(true);
            SetupResult.For(column.Table).Return(table);

            mocks.ReplayAll();

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
            IIndex index = CreateIndex(false);

            mocks.ReplayAll();

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

            IColumns columns = mocks.Stub<IColumns>();
            IColumn col0 = mocks.Stub<IColumn>();
            IColumn col1 = mocks.Stub<IColumn>();

            SetupResult.For(columns[0]).Return(col0);
            SetupResult.For(columns[1]).Return(col1);
            SetupResult.For(columns.Count).Return(2);

            SetupResult.For(col0.Name).Return("Name");
            SetupResult.For(col0.IsNullable).Return(false);
            SetupResult.For(col0.DataTypeNameComplete).Return("NVARCHAR(50)");

            SetupResult.For(col1.Name).Return("Dob");
            SetupResult.For(col1.IsNullable).Return(true);
            SetupResult.For(col1.DataTypeNameComplete).Return("NVARCHAR(50)");

            SetupResult.For(table.Columns).Return(columns);

            mocks.ReplayAll();

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
            IIndex index = CreateIndex(true);

            mocks.ReplayAll();

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

            IColumn column = mocks.Stub<IColumn>();
            SetupResult.For(column.Name).Return("LastName");
            SetupResult.For(column.Table).Return(table);

            mocks.ReplayAll();

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

            IColumn fkColumn = mocks.Stub<IColumn>();
            SetupResult.For(fkColumn.Name).Return("BillingID");
            SetupResult.For(fkColumn.Table).Return(table);

            IColumns foreignColumns = mocks.Stub<IColumns>();
            SetupResult.For(foreignColumns[0]).Return(fkColumn);
            SetupResult.For(foreignColumns.Count).Return(1);

            IForeignKey fk = mocks.Stub<IForeignKey>();
            SetupResult.For(fk.ForeignColumns).Return(foreignColumns);
            SetupResult.For(fk.Name).Return("FK_Customer_BillingID");

            mocks.ReplayAll();

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