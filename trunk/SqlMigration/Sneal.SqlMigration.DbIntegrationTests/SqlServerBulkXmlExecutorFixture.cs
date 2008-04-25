using System;
using System.Configuration;
using MyMeta;
using NUnit.Framework;
using Sneal.SqlMigration.Impl;
using Sneal.SqlMigration.Migrators;

namespace Sneal.SqlMigration.DbIntegrationTests
{
    [TestFixture]
    public class SqlServerBulkXmlExecutorFixture
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            string server = ConfigurationManager.AppSettings["server"];
            string database1 = ConfigurationManager.AppSettings["database1"];
            connSettings = new SqlServerConnectionSettings(server, database1);

            // TODO: remove this
            connSettings.UserName = "sa";
            connSettings.Password = "disk44you";

            customerXmlPath = AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Scripts\Customer.xml";
        }

        #endregion

        private SqlServerBulkXmlExecutor executor;
        private string customerXmlPath;
        private SqlServerConnectionSettings connSettings;

        [Test]
        public void ShouldLoadCustomerXmlToTable()
        {
            IDatabase db = DatabaseConnectionFactory.CreateDbConnection(connSettings);
            ITable table = db.Tables["Customer"];

            Assert.IsNotNull(table, "Could not get table from database");

            executor = new SqlServerBulkXmlExecutor();
            executor.LoadTable(table, customerXmlPath);

            // we didn't throw, that's pretty good... ;-)
        }
    }
}