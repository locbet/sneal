namespace Sneal.SqlMigration.Tests.TestHelpers
{
    internal static class DomainGenerator
    {
        internal static DatabaseStub CreateStubbedSourceDB()
        {
            DatabaseStub db = new DatabaseStub("AdeventureWorks");

            TableStub table = new TableStub(db, "Customer");
            table.columns.Add(new ColumnStub(table, "CustomerID", "INT"));
            table.columns.Add(new ColumnStub(table, "FirstName", "NVARCHAR(50)"));
            table.columns.Add(new ColumnStub(table, "LastName", "NVARCHAR(50)"));
            table.columns.Add(new ColumnStub(table, "Email", "NVARCHAR(50)"));
            table.columns.Add(new ColumnStub(table, "PrimaryEmail", "NVARCHAR(50)"));
            table.columns.Add(new ColumnStub(table, "CustomerTypeID", "INT"));

            return db;
        }

        internal static DatabaseStub CreateStubbedTargetDB()
        {
            DatabaseStub db = new DatabaseStub("AdeventureWorks");

            TableStub table = new TableStub(db, "Customer");
            table.columns.Add(new ColumnStub(table, "CustomerID", "INT"));
            table.columns.Add(new ColumnStub(table, "FirstName", "NVARCHAR(50)"));
            table.columns.Add(new ColumnStub(table, "LastName", "NVARCHAR(50)"));
            table.columns.Add(new ColumnStub(table, "Email", "NVARCHAR(50)"));
            table.columns.Add(new ColumnStub(table, "CustomerTypeID", "INT"));
            table.columns.Add(new ColumnStub(table, "CountryID", "INT"));

            return db;
        }
    }
}