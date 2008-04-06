using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using NUnit.Framework;
using Sneal.SqlMigration.Impl;

namespace Sneal.SqlMigration.DbIntegrationTests
{
    [TestFixture]
    public class ScriptDataDiffFixture
    {
        private MigrationEngine engine;
        private SqlServerConnectionSettings connSettings1;
        private SqlServerConnectionSettings connSettings2;
        private string exportDir = "c:\\temp";

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

            string database1 = ConfigurationManager.AppSettings["database1"];
            string database2 = ConfigurationManager.AppSettings["database2"];

            connSettings1 = new SqlServerConnectionSettings(server, database1);
            connSettings2 = new SqlServerConnectionSettings(server, database2);
        }

        //[Test]
        public void ScriptData()
        {
            IList<DbObjectName> tables = engine.GetAllTables(connSettings1);

            ScriptingOptions options = new ScriptingOptions();
            options.ExportDirectory = exportDir;
            options.ScriptData = true;
            options.AddTablesToScript(tables);

            engine.Script(connSettings1, options);
        }

        [Test]
        public void ShouldScriptCustomerDataDifferences()
        {
            IList<DbObjectName> tables = engine.GetAllTables(connSettings1);

            ScriptingOptions options = new ScriptingOptions();
            options.ExportDirectory = exportDir;
            options.ScriptData = true;
            options.AddTablesToScript(tables);

            engine.ScriptDifferences(connSettings1, connSettings2, options);

            string dataPath = Path.Combine(exportDir, "\\Data");
            Assert.IsTrue(File.Exists(Path.Combine(dataPath, "dbo.Customer.sql")));
            Assert.IsTrue(File.Exists(Path.Combine(dataPath, "dbo.Address.sql")));
            Assert.IsTrue(File.Exists(Path.Combine(dataPath, "dbo.SoftwareVersion.sql")));
        }
    }
}
