using System;
using System.Collections.Generic;
using System.Configuration;
using NUnit.Framework;
using Sneal.SqlMigration.Impl;

namespace Sneal.SqlMigration.AdventureWorksTests
{
    [TestFixture]
    public class SchemaFixture
    {
        private MigrationEngine engine;
        private SqlServerConnectionSettings connectionSettings;

        [SetUp]
        public void SetUp()
        {
            engine = new MigrationEngine(new SqlServerScriptBuilder(), new DatabaseComparer());
            
            ScriptMessageManager msgMgr = new ScriptMessageManager();
            engine.MessageManager = msgMgr;
            msgMgr.ScriptMessage += delegate(object src, ScriptMessageEventArgs e)
                {
                    Console.WriteLine(e.Message);
                };

            string server = ConfigurationManager.AppSettings["server"];
            string database = ConfigurationManager.AppSettings["database"];
            connectionSettings = new SqlServerConnectionSettings(server, database);
            connectionSettings.UserName = "sa";
            connectionSettings.Password = "disk44you";
        }

        [Test]
        public void ShouldGetAllTables()
        {
            IList<DbObjectName> tables = engine.GetAllTables(connectionSettings);
            Assert.That(tables.Count == 66);
        }

        [Test]
        public void ShouldGetAllViews()
        {
            IList<DbObjectName> views = engine.GetAllViews(connectionSettings);
            Assert.That(views.Count == 0);
        }

        [Test]
        public void ShouldGetAllSprocs()
        {
            IList<DbObjectName> sprocs = engine.GetAllSprocs(connectionSettings);
            Assert.That(sprocs.Count == 0);
        }

        [Test]
        public void ShouldScriptCustomerDataDifferences()
        {
            
        }
    }
}