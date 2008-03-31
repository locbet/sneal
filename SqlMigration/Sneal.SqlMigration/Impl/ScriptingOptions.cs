using System.Collections.Generic;

namespace Sneal.SqlMigration.Impl
{
    public class ScriptingOptions : IScriptingOptions
    {
        private bool scriptData;
        private bool scriptConstraints;
        private bool scriptIndexes;
        private bool scriptSchema;
        private readonly IList<string> sprocsToScript = new List<string>();
        private readonly IList<string> tablesToScript = new List<string>();
        private bool useMultipleFiles;
        private readonly IList<string> viewsToScript = new List<string>();
        private string exportDirectory;

        #region IScriptingOptions Members

        public bool ScriptConstraints
        {
            get { return scriptConstraints; }
            set { scriptConstraints = value; }
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
            set { exportDirectory = value; }
        }

        public bool UseMultipleFiles
        {
            get { return useMultipleFiles; }
            set { useMultipleFiles = value; }
        }

        public IList<string> SprocsToScript
        {
            get { return sprocsToScript; }
        }

        public IList<string> TablesToScript
        {
            get { return tablesToScript; }
        }

        public IList<string> ViewsToScript
        {
            get { return viewsToScript; }
        }

        #endregion
    }
}