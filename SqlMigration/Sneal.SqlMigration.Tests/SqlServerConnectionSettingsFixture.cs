using NUnit.Framework;
using Sneal.SqlMigration.Impl;

namespace Sneal.SqlMigration.Tests
{
    [TestFixture]
    public class SqlServerConnectionSettingsFixture
    {
        [Test]
        public void ShouldCreateAdventureWorksConnectionStringOnLocalHost()
        {
            SqlServerConnectionSettings s = new SqlServerConnectionSettings("localhost", "AdventureWorks");
            s.Password = "PASS";
            s.UserName = "me";
            s.UseIntegratedAuthentication = false;

            Assert.AreEqual("localhost", s.ServerName);
            Assert.AreEqual("AdventureWorks", s.Database);
            Assert.AreEqual("SQL", s.DriverType);
            Assert.AreEqual("Provider=SQLOLEDB.1; Data Source=localhost; Initial Catalog=AdventureWorks; User ID=me; Password=PASS", s.ConnectionString);
        }

        [Test]
        [ExpectedException(typeof(SqlMigrationException))]
        public void ShouldThrowIfServerIsNotSet()
        {
            SqlServerConnectionSettings s = new SqlServerConnectionSettings();
            s.Database = "Northwind";
            string temp = s.ConnectionString;
        }

        [Test]
        [ExpectedException(typeof(SqlMigrationException))]
        public void ShouldThrowIfDatabaseIsNotSet()
        {
            SqlServerConnectionSettings s = new SqlServerConnectionSettings();
            s.ServerName = "localhost";
            string temp = s.ConnectionString;
        }
    }
}