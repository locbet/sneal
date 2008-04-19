using System.Collections.Generic;
using Sneal.Preconditions;

namespace Sneal.SqlMigration.Impl
{
    public class ScriptingOptions : IScriptingOptions
    {
        private readonly IList<DbObjectName> sprocsToScript = new List<DbObjectName>();
        private readonly IList<DbObjectName> tablesToScript = new List<DbObjectName>();
        private readonly IList<DbObjectName> viewsToScript = new List<DbObjectName>();
        private string exportDirectory;
        private bool scriptData;
        private bool scriptDataAsXml;
        private bool scriptForeignKeys;
        private bool scriptIndexes;
        private bool scriptSchema;
        private bool useMultipleFiles = true;

        #region IScriptingOptions Members

        public bool ScriptDataAsXml
        {
            get { return scriptDataAsXml; }
            set { scriptDataAsXml = value; }
        }

        public bool ScriptForeignKeys
        {
            get { return scriptForeignKeys; }
            set { scriptForeignKeys = value; }
        }

        public bool ScriptIndexes
        {
            get { return scriptIndexes; }
            set { scriptIndexes = value; }
        }

        public bool ScriptSchema
        {
            get { return scriptSchema; }
            set { scriptSchema = value; }
        }

        public bool ScriptData
        {
            get { return scriptData; }
            set { scriptData = value; }
        }

        public string ExportDirectory
        {
            get { return exportDirectory; }
            set
            {
                Throw.If(value, "ExportDirectory").IsEmpty();
                exportDirectory = value;
            }
        }

        public bool UseMultipleFiles
        {
            get { return useMultipleFiles; }
            set { useMultipleFiles = value; }
        }

        public IList<DbObjectName> SprocsToScript
        {
            get { return sprocsToScript; }
        }

        public IList<DbObjectName> TablesToScript
        {
            get { return tablesToScript; }
        }

        public IList<DbObjectName> ViewsToScript
        {
            get { return viewsToScript; }
        }

        #endregion

        public void AddSprocToScript(DbObjectName name)
        {
            sprocsToScript.Add(name);
        }

        public void AddViewToScript(DbObjectName name)
        {
            viewsToScript.Add(name);
        }

        public void AddTableToScript(DbObjectName name)
        {
            tablesToScript.Add(name);
        }

        public void AddSprocsToScript(IEnumerable<DbObjectName> sprocs)
        {
            foreach (DbObjectName name in sprocs)
            {
                AddSprocToScript(name);
            }
        }

        public void AddViewsToScript(IEnumerable<DbObjectName> views)
        {
            foreach (DbObjectName name in views)
            {
                AddViewToScript(name);
            }
        }

        public void AddTablesToScript(IEnumerable<DbObjectName> tables)
        {
            foreach (DbObjectName name in tables)
            {
                AddTableToScript(name);
            }
        }
    }
}