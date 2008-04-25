using System.Collections.Generic;
using System.Reflection;
using Sneal.CmdLineParser;

namespace Sneal.SqlMigration.Console
{
    public class CmdLineScriptingOptions : IScriptingOptions
    {
        private readonly IList<DbObjectName> sprocsToScript = new List<DbObjectName>();
        private readonly IList<DbObjectName> tablesToScript = new List<DbObjectName>();
        private readonly IList<DbObjectName> viewsToScript = new List<DbObjectName>();
        private readonly IList<IScriptFile> executorScripts = new List<IScriptFile>();
        private string exportDirectory = Assembly.GetExecutingAssembly().Location;
        private string log4netConfigPath;
        private bool scriptData;
        private bool scriptDataAsXml;
        private bool scriptForeignKeys;
        private bool scriptIndexes;
        private bool scriptSchema;
        private bool showHelp;
        private bool useMultipleFiles = true;

        [Switch("log4net", "Specifies the file path to the log4net configuration file to use.")]
        public string Log4netConfigPath
        {
            get { return log4netConfigPath; }
            set { log4netConfigPath = value; }
        }

        [Switch("help", "Shows the command line help.")]
        public bool ShowHelp
        {
            get { return showHelp; }
            set { showHelp = value; }
        }

        [Switch("execute", "Comma or semi-colon delimited list of scripts to run or xml files to load.")]
        public IList<IScriptFile> ExecutorScripts
        {
            get { return executorScripts; }
        }

        #region IScriptingOptions Members

        [Switch("fks", "Scripts table foreign key constraints for each table specified.")]
        public bool ScriptForeignKeys
        {
            get { return scriptForeignKeys; }
            set { scriptForeignKeys = value; }
        }

        [Switch("idx", "Script table indexes for each table specified.")]
        public bool ScriptIndexes
        {
            get { return scriptIndexes; }
            set { scriptIndexes = value; }
        }

        [Switch("ddl", "Table DDL is scripted for each table specified.")]
        public bool ScriptSchema
        {
            get { return scriptSchema; }
            set { scriptSchema = value; }
        }

        [Switch("data", "Table data is scripted for each table specified.")]
        public bool ScriptData
        {
            get { return scriptData; }
            set { scriptData = value; }
        }

        [Switch("xmldata", "Table data is scripted for each table specified as XML.")]
        public bool ScriptDataAsXml
        {
            get { return scriptDataAsXml; }
            set { scriptDataAsXml = value; }
        }

        [Switch("dir", "The root directory to write scripts to, the default is current dir.")]
        public string ExportDirectory
        {
            get { return exportDirectory; }
            set { exportDirectory = value; }
        }

        [Switch("multiplefiles", "If specified, each logical object component is written to a separate file.")]
        public bool UseMultipleFiles
        {
            get { return useMultipleFiles; }
            set { useMultipleFiles = value; }
        }

        [Switch("sprocs", "Comma or semi-colon delimited list of stored procedures to script.")]
        public IList<DbObjectName> SprocsToScript
        {
            get { return sprocsToScript; }
        }

        [Switch("tables", "Comma or semi-colon delimited list of tables to script DDL or data for.")]
        public IList<DbObjectName> TablesToScript
        {
            get { return tablesToScript; }
        }

        [Switch("views", "Comma or semi-colon delimited list of views to script.")]
        public IList<DbObjectName> ViewsToScript
        {
            get { return viewsToScript; }
        }

        #endregion
    }
}