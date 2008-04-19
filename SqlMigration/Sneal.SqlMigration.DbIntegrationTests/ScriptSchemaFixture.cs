using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using NUnit.Framework;
using Sneal.SqlMigration.Impl;

namespace Sneal.SqlMigration.DbIntegrationTests
{
    [TestFixture]
    public class ScriptSchemaFixture
    {
        private MigrationEngine engine;
        private SqlServerConnectionSettings connSettings;
        private string exportDir;

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

            exportDir = ConfigurationManager.AppSettings["exportdir"];
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

            string dataPath = Path.Combine(exportDir, "Schema\\Tables");
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

            string dataPath = Path.Combine(exportDir, "Schema\\Indexes");
            Assert.IsTrue(File.Exists(Path.Combine(dataPath, "dbo.Customer.sql")), "customer script missing");
            Assert.IsTrue(File.Exists(Path.Combine(dataPath, "dbo.Country.sql")), "country script missing");
            Assert.IsTrue(File.Exists(Path.Combine(dataPath, "dbo.Address.sql")), "address script missing");
            Assert.IsTrue(File.Exists(Path.Combine(dataPath, "dbo.SoftwareVersion.sql")), "software script missing");
        }

        [Test]
        public void ShouldScriptAllTableFKs()
        {
            IList<DbObjectName> tables = engine.GetAllTables(connSettings);

            ScriptingOptions options = new ScriptingOptions();
            options.ExportDirectory = exportDir;
            options.ScriptForeignKeys = true;
            options.AddTablesToScript(tables);

            engine.Script(connSettings, options);

            string dataPath = Path.Combine(exportDir, "Schema\\ForeignKeys");
            Assert.IsTrue(File.Exists(Path.Combine(dataPath, "dbo.Address.sql")), "address script missing");
        }

        [Test]
        public void ShouldScriptAllSprocs()
        {
            IList<DbObjectName> sprocs = engine.GetAllSprocs(connSettings);

            ScriptingOptions options = new ScriptingOptions();
            options.ExportDirectory = exportDir;
            options.AddSprocsToScript(sprocs);

            engine.Script(connSettings, options);

            string dataPath = Path.Combine(exportDir, "Sprocs");
            Assert.IsTrue(File.Exists(Path.Combine(dataPath, "dbo.GetCustomers.sql")), "GetCustomers script missing");
            Assert.IsTrue(File.Exists(Path.Combine(dataPath, "dbo.GetCustomerAddress.sql")), "GetCustomerAddress script missing");
        }

        [Test]
        public void ShouldScriptAllViews()
        {
            IList<DbObjectName> views = engine.GetAllViews(connSettings);

            ScriptingOptions options = new ScriptingOptions();
            options.ExportDirectory = exportDir;
            options.AddViewsToScript(views);

            engine.Script(connSettings, options);

            string dataPath = Path.Combine(exportDir, "Views");
            Assert.IsTrue(File.Exists(Path.Combine(dataPath, "dbo.CustomerAndPrimaryAddress.sql")), "CustomerAndPrimaryAddress script missing");
        }

        [Test]
        public void ShouldScriptEverythingToSingleScript()
        {
            IList<DbObjectName> views = engine.GetAllViews(connSettings);
            IList<DbObjectName> sprocs = engine.GetAllSprocs(connSettings);
            IList<DbObjectName> tables = engine.GetAllTables(connSettings);

            ScriptingOptions options = new ScriptingOptions();
            options.ExportDirectory = exportDir;
            options.AddTablesToScript(tables);
            options.AddSprocsToScript(sprocs);
            options.AddViewsToScript(views);
            options.ScriptData = true;
            options.ScriptForeignKeys = true;
            options.ScriptIndexes = true;
            options.ScriptSchema = true;
            options.UseMultipleFiles = false;

            engine.Script(connSettings, options);

            Assert.IsTrue(File.Exists(Path.Combine(exportDir, "SqlMigration1.sql")));
        }
        
    }
}
