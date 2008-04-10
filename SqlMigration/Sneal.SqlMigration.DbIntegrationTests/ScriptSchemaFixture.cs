using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using NUnit.Framework;
using Sneal.SqlMigration.Impl;

namespace Sneal.SqlMigration.DbIntegrationTests
{
    [TestFixture]
    public class ScriptSchemaFixture
    {
        private MigrationEngine engine;
        private SqlServerConnectionSettings connSettings;
        private string exportDir = @"c:\temp";

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
            connSettings = new SqlServerConnectionSettings(server, database1);

            // TODO: remove this
            connSettings.UserName = "sa";
            connSettings.Password = "disk44you";
            connSettings.UseIntegratedAuthentication = false;
        }

        [Test]
        public void ShouldScriptAllTables()
        {
            IList<DbObjectName> tables = engine.GetAllTables(connSettings);

            ScriptingOptions options = new ScriptingOptions();
            options.ExportDirectory = exportDir;
            options.ScriptSchema = true;
            options.AddTablesToScript(tables);

            engine.Script(connSettings, options);

            string dataPath = Path.Combine(exportDir, "Table");
            Assert.IsTrue(File.Exists(Path.Combine(dataPath, "dbo.Customer.sql")), "customer script missing");
            Assert.IsTrue(File.Exists(Path.Combine(dataPath, "dbo.Country.sql")), "country script missing");
            Assert.IsTrue(File.Exists(Path.Combine(dataPath, "dbo.Address.sql")), "address script missing");
            Assert.IsTrue(File.Exists(Path.Combine(dataPath, "dbo.SoftwareVersion.sql")), "software script missing");
        }

        [Test]
        public void ShouldScriptAllTableIndexes()
        {
            IList<DbObjectName> tables = engine.GetAllTables(connSettings);

            ScriptingOptions options = new ScriptingOptions();
            options.ExportDirectory = exportDir;
            options.ScriptIndexes = true;
            options.AddTablesToScript(tables);

            engine.Script(connSettings, options);

            string dataPath = Path.Combine(exportDir, "Index");
            Assert.IsTrue(File.Exists(Path.Combine(dataPath, "dbo.Customer.sql")), "customer script missing");
            Assert.IsTrue(File.Exists(Path.Combine(dataPath, "dbo.Country.sql")), "country script missing");
            Assert.IsTrue(File.Exists(Path.Combine(dataPath, "dbo.Address.sql")), "address script missing");
            Assert.IsTrue(File.Exists(Path.Combine(dataPath, "dbo.SoftwareVersion.sql")), "software script missing");
        }
        
        // NOTE: Data scripting seems OK
        // NOTE: Indexes (PK and nonclustered seem OK)
        // NOTE: Verified table DDL scripts are good.
        // NOTE: scripting defaults, done.
        // TODO: Scipting FKs
        // TODO: Scripting sprocs and views need to be tested and verified.
    }
}
