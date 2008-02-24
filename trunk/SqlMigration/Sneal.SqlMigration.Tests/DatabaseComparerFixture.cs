using NUnit.Framework;
using Sneal.SqlMigration.Impl;
using Sneal.SqlMigration.Tests.TestHelpers;

namespace Sneal.SqlMigration.Tests
{
    [TestFixture]
    public class DatabaseComparerFixture
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            comparer = new DatabaseComparer();
        }

        #endregion

        private DatabaseComparer comparer;

        [Test]
        public void ShouldReturnColumnDoesNotExist()
        {
            DatabaseStub srcDB = new DatabaseStub("SourceDB");
            TableStub srcTable = srcDB.AddStubbedTable("Table1");
            srcTable.AddStubbedColumn("Col1", "INT");

            DatabaseStub targetDB = new DatabaseStub("TargetDB");
            targetDB.AddStubbedTable("Table1");

            Assert.IsFalse(comparer.Column(srcTable.Columns[0]).ExistsIn(targetDB));
        }

        [Test]
        public void ShouldReturnColumnDoesNotExistWhenTableDoesNotExist()
        {
            DatabaseStub srcDB = new DatabaseStub("SourceDB");
            TableStub srcTable = srcDB.AddStubbedTable("Table1");
            srcTable.AddStubbedColumn("Col1", "INT");

            DatabaseStub targetDB = new DatabaseStub("TargetDB");

            Assert.IsFalse(comparer.Column(srcTable.Columns[0]).ExistsIn(targetDB));
        }

        [Test]
        public void ShouldReturnColumnExists()
        {
            DatabaseStub srcDB = new DatabaseStub("SourceDB");
            TableStub srcTable = srcDB.AddStubbedTable("Table1");
            srcTable.AddStubbedColumn("Col1", "INT");

            DatabaseStub targetDB = new DatabaseStub("TargetDB");
            TableStub targetTable = targetDB.AddStubbedTable("Table1");
            targetTable.AddStubbedColumn("Col1", "INT");

            Assert.IsTrue(comparer.Column(srcTable.Columns[0]).ExistsIn(targetDB));
        }

        [Test]
        public void ShouldReturnForeignKeyDoesNotExists()
        {
            DatabaseStub srcDB = new DatabaseStub("SourceDB");
            TableStub srcTable = srcDB.AddStubbedTable("Table1");
            ColumnStub srcCol = srcTable.AddStubbedColumn("Col1", "INT");
            srcCol.AddForeignKeyStub("FK1");

            DatabaseStub targetDB = new DatabaseStub("TargetDB");
            TableStub targetTable = targetDB.AddStubbedTable("Table1");
            ColumnStub targetCol = targetTable.AddStubbedColumn("Col1", "INT");
            targetCol.AddForeignKeyStub("FK2");

            Assert.IsFalse(comparer.ForeignKey(srcTable.ForeignKeys[0]).ExistsIn(targetDB));
        }

        [Test]
        public void ShouldReturnForeignKeyDoesNotExistWhenTableDoesNotExist()
        {
            DatabaseStub srcDB = new DatabaseStub("SourceDB");
            TableStub srcTable = srcDB.AddStubbedTable("Table1");
            ColumnStub srcCol = srcTable.AddStubbedColumn("Col1", "INT");
            srcCol.AddForeignKeyStub("FK1");

            DatabaseStub targetDB = new DatabaseStub("TargetDB");

            Assert.IsFalse(comparer.ForeignKey(srcTable.ForeignKeys[0]).ExistsIn(targetDB));
        }

        [Test]
        public void ShouldReturnForeignKeyExists()
        {
            DatabaseStub srcDB = new DatabaseStub("SourceDB");
            TableStub srcTable = srcDB.AddStubbedTable("Table1");
            ColumnStub srcCol = srcTable.AddStubbedColumn("Col1", "INT");
            srcCol.AddForeignKeyStub("FK1");

            DatabaseStub targetDB = new DatabaseStub("TargetDB");
            TableStub targetTable = targetDB.AddStubbedTable("Table1");
            ColumnStub targetCol = targetTable.AddStubbedColumn("Col1", "INT");
            targetCol.AddForeignKeyStub("FK1");

            Assert.IsTrue(comparer.ForeignKey(srcTable.ForeignKeys[0]).ExistsIn(targetDB));
        }

        [Test]
        public void ShouldReturnTableDoesNotExist()
        {
            DatabaseStub srcDB = new DatabaseStub("SourceDB");
            TableStub srcTable = srcDB.AddStubbedTable("Table1");

            DatabaseStub targetDB = new DatabaseStub("TargetDB");

            Assert.IsFalse(comparer.Table(srcTable).ExistsIn(targetDB));
        }

        [Test]
        public void ShouldReturnTableExists()
        {
            DatabaseStub srcDB = new DatabaseStub("SourceDB");
            TableStub srcTable = srcDB.AddStubbedTable("Table1");

            DatabaseStub targetDB = new DatabaseStub("TargetDB");
            targetDB.AddStubbedTable("Table1");

            Assert.IsTrue(comparer.Table(srcTable).ExistsIn(targetDB));
        }

        [Test]
        public void ShouldReturnIndexDoesNotExists()
        {
            DatabaseStub srcDB = new DatabaseStub("SourceDB");
            TableStub srcTable = srcDB.AddStubbedTable("Table1");
            ColumnStub srcCol = srcTable.AddStubbedColumn("Col1", "INT");
            srcTable.AddStubbedIndex(srcCol, "IX_COL1");

            DatabaseStub targetDB = new DatabaseStub("TargetDB");
            TableStub targetTable = targetDB.AddStubbedTable("Table1");
            ColumnStub targetCol = targetTable.AddStubbedColumn("Col2", "INT");
            targetTable.AddStubbedIndex(targetCol, "IX_COL2");

            Assert.IsFalse(comparer.Index(srcTable.Indexes[0]).ExistsIn(targetDB));
        }

        [Test]
        public void ShouldReturnIndexDoesNotExistWhenTableDoesNotExist()
        {
            DatabaseStub srcDB = new DatabaseStub("SourceDB");
            TableStub srcTable = srcDB.AddStubbedTable("Table1");
            ColumnStub srcCol = srcTable.AddStubbedColumn("Col1", "INT");
            srcTable.AddStubbedIndex(srcCol, "IX_COL1");

            DatabaseStub targetDB = new DatabaseStub("TargetDB");

            Assert.IsFalse(comparer.Index(srcTable.Indexes[0]).ExistsIn(targetDB));
        }

        [Test]
        public void ShouldReturnIndexExists()
        {
            DatabaseStub srcDB = new DatabaseStub("SourceDB");
            TableStub srcTable = srcDB.AddStubbedTable("Table1");
            ColumnStub srcCol = srcTable.AddStubbedColumn("Col1", "INT");
            srcTable.AddStubbedIndex(srcCol, "IX_COL1");

            DatabaseStub targetDB = new DatabaseStub("TargetDB");
            TableStub targetTable = targetDB.AddStubbedTable("Table1");
            ColumnStub targetCol = targetTable.AddStubbedColumn("Col1", "INT");
            targetTable.AddStubbedIndex(targetCol, "IX_COL1");

            Assert.IsTrue(comparer.Index(srcTable.Indexes[0]).ExistsIn(targetDB));
        }
    }
}