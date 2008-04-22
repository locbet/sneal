using System.Data;
using NUnit.Framework;
using Sneal.SqlMigration.Impl;
using Sneal.SqlMigration.Tests.TestHelpers;

namespace Sneal.SqlMigration.Tests
{
    [TestFixture]
    public class TableDataFixture
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            table = new TableStub(new DatabaseStub("Test"), "Customers");
            table.AddStubbedColumn("CustomerID", "INT");
            table.AddStubbedColumn("FirstName", "NVARCHAR(50)");
            table.AddStubbedColumn("DateOfBirth", "DATETIME");

            tblData = new TableData();
        }

        #endregion

        private TableStub table;
        private TableData tblData;

        [Test]
        public void ShouldBuildSelectStatement()
        {
            string sql = tblData.BuildTableSelectStatement(table);
            Assert.AreEqual("SELECT [CustomerID], [FirstName], [DateOfBirth] FROM Customers", sql);
        }

        [Test]
        public void ShouldGetEscapedSqlString()
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("FirstName", typeof (string));
            dataTable.Rows.Add("Shawn");

            string val = tblData.GetSqlColumnValue(table.Columns[1], dataTable.Rows[0]);
            Assert.AreEqual("'Shawn'", val);
        }
    }
}