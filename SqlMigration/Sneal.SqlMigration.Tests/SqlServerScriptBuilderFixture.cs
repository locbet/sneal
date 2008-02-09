using System;
using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using Sneal.SqlMigration.Impl;

namespace Sneal.SqlMigration.Tests
{
    [TestFixture]
    public class SqlServerScriptBuilderFixture
    {
        private SqlServerScriptBuilder scriptBuilder;
        private ISqlTemplateManager templateManager;
        private MockRepository mocks;
        private ITable table;
        private string templateDir;

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


        [Test]
        public void ShouldScriptCreateTable()
        {
            SetupResult.For(templateManager.CreateTable).Return(@"CreateTable.vm");

            IColumn[] columns = new IColumn[2];
            columns[0] = mocks.Stub<IColumn>();
            columns[1] = mocks.Stub<IColumn>();

            SetupResult.For(columns[0].Name).Return("Name");
            SetupResult.For(columns[0].IsNullable).Return(false);
            SetupResult.For(columns[0].DataType).Return(SqlDataType.NVarChar);

            SetupResult.For(columns[1].Name).Return("Dob");
            SetupResult.For(columns[1].IsNullable).Return(true);
            SetupResult.For(columns[1].DataType).Return(SqlDataType.DateTime);
            
            SetupResult.For(table.Columns).Return(columns);

            mocks.ReplayAll();

            SqlScript script = scriptBuilder.Create(table);
            string sql = script.ToScript();

            Console.WriteLine(sql);
            Assert.IsTrue(sql.Contains("IF NOT EXISTS"), "Missing IF NOT EXISTS");
            Assert.IsTrue(sql.Contains("CREATE TABLE [dbo].[Customer]"));
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

        [Test]
        public void ShouldScriptAddColumn()
        {
            SetupResult.For(templateManager.CreateColumn).Return(@"AddColumn.vm");

            IColumn column = mocks.Stub<IColumn>();

            SetupResult.For(column.Name).Return("LastName");
            SetupResult.For(column.IsNullable).Return(false);
            SetupResult.For(column.DataType).Return(SqlDataType.NVarChar);
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
        public void ShouldScriptAlterColumn()
        {
            SetupResult.For(templateManager.AlterColumn).Return(@"AlterColumn.vm");

            IColumn column = mocks.Stub<IColumn>();
            SetupResult.For(column.Schema).Return("dbo");
            SetupResult.For(column.Name).Return("LastName");
            SetupResult.For(column.DataType).Return(SqlDataType.NVarChar);
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
        public void ShouldScriptAddForeignKey()
        {
            SetupResult.For(templateManager.CreateForeignKey).Return(@"AddForeignKey.vm");

            ITable pkTable = mocks.Stub<ITable>();
            SetupResult.For(pkTable.Name).Return("Billing");
            SetupResult.For(pkTable.Schema).Return("dbo");

            IColumn fkColumn = mocks.Stub<IColumn>();
            SetupResult.For(fkColumn.Name).Return("BillingID");
            SetupResult.For(fkColumn.IsNullable).Return(true);
            SetupResult.For(fkColumn.DataType).Return(SqlDataType.Int);
            SetupResult.For(fkColumn.Table).Return(table);

            IColumn pkColumn = mocks.Stub<IColumn>();
            SetupResult.For(pkColumn.Name).Return("BillingID");
            SetupResult.For(pkColumn.IsNullable).Return(true);
            SetupResult.For(pkColumn.DataType).Return(SqlDataType.Int);
            SetupResult.For(pkColumn.Table).Return(pkTable);

            IForeignKey fk = mocks.Stub<IForeignKey>();
            SetupResult.For(fk.ForeignKeyColumn).Return(fkColumn);    
            SetupResult.For(fk.PrimaryKeyColumn).Return(pkColumn);
            SetupResult.For(fk.Name).Return("FK_Customer_BillingID");
            SetupResult.For(fk.Schema).Return("dbo"); 

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
        public void ShouldScriptDropForeignKey()
        {
            SetupResult.For(templateManager.DropForeignKey).Return(@"DropForeignKey.vm");

            IColumn fkColumn = mocks.Stub<IColumn>();
            SetupResult.For(fkColumn.Name).Return("BillingID");
            SetupResult.For(fkColumn.Table).Return(table);

            IForeignKey fk = mocks.Stub<IForeignKey>();
            SetupResult.For(fk.ForeignKeyColumn).Return(fkColumn);
            SetupResult.For(fk.Name).Return("FK_Customer_BillingID");
            SetupResult.For(fk.Schema).Return("dbo");

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
        public void ShouldScriptCreateUniqueIndex()
        {
            SetupResult.For(templateManager.CreateIndex).Return(@"CreateIndex.vm");
            IIndex index = CreateIndex(true);

            mocks.ReplayAll();

            SqlScript script = scriptBuilder.Create(index);
            string sql = script.ToScript();

            Console.WriteLine(sql);
            Assert.IsTrue(sql.Contains("IF NOT EXISTS"), "Missing IF NOT EXISTS");
            Assert.IsTrue(sql.Contains("CREATE UNIQUE NONCLUSTERED INDEX [IX_LastName]"), "Missing CREATE NONCLUSTERED INDEX");
            Assert.IsTrue(sql.Contains("[dbo].[Customer]"), "Missing [dbo].[Customer]");
            Assert.IsTrue(sql.Contains("LastName"), "Missing LastName");
        }

        private IIndex CreateIndex(bool isUnique)
        {
            IColumn column = mocks.Stub<IColumn>();
            SetupResult.For(column.Name).Return("LastName");
            SetupResult.For(column.IsNullable).Return(false);
            SetupResult.For(column.DataType).Return(SqlDataType.NVarChar);
            SetupResult.For(column.Table).Return(table);
            List<IColumn> columns = new List<IColumn>();
            columns.Add(column);

            IIndex index = mocks.Stub<IIndex>();
            SetupResult.For(index.Columns).Return(columns);
            SetupResult.For(index.Schema).Return("dbo");
            SetupResult.For(index.Name).Return("IX_LastName");
            SetupResult.For(index.IsUnique).Return(isUnique);
            SetupResult.For(index.Table).Return(table);

            return index;
        }
    }
}
	