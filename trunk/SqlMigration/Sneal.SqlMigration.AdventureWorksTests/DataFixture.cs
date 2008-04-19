using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using NUnit.Framework;
using Sneal.SqlMigration.Impl;

namespace Sneal.SqlMigration.AdventureWorksTests
{
    [TestFixture]
    public class DataFixture
    {
        private MigrationEngine engine;
        private SqlServerConnectionSettings connectionSettings;
        private ScriptingOptions scriptingOptions;

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
            connectionSettings.UseIntegratedAuthentication = false;
            connectionSettings.UserName = "sa";
            connectionSettings.Password = "disk44you";
            
            scriptingOptions = new ScriptingOptions();
            scriptingOptions.ScriptData = true;
            scriptingOptions.UseMultipleFiles = true;
            scriptingOptions.ExportDirectory = ConfigurationManager.AppSettings["exportdir"];
        }

        [Test]
        public void ShouldScriptCustomerDataAsXml()
        {
            scriptingOptions.ScriptData = false;
            scriptingOptions.ScriptDataAsXml = true;
            scriptingOptions.AddTableToScript("dbo.Customer");
            engine.Script(connectionSettings, scriptingOptions);

            string dataPath = Path.Combine(scriptingOptions.ExportDirectory, "Data");
            Assert.IsTrue(File.Exists(Path.Combine(dataPath, "dbo.CustomerAndPrimaryAddress.xml")));
        }

        [Test]
        public void ShouldScriptContactTable()
        {
            scriptingOptions.AddTableToScript("dbo.Contact");
            engine.Script(connectionSettings, scriptingOptions);
        }

        [Test]
        public void ShouldNotWriteEmptyTable()
        {
            scriptingOptions.AddTableToScript("dbo.WorkOrderRouting");
            engine.Script(connectionSettings, scriptingOptions);
        }

        [Test, Ignore("This is a long test, this scripts ALL data in AdventureWorks.")]
        public void ShouldScriptAllTables()
        {
            IList<DbObjectName> tables = engine.GetAllTables(connectionSettings);
            scriptingOptions.AddTablesToScript(tables);
            engine.Script(connectionSettings, scriptingOptions);
        }
    }
}
