using System.Collections.Generic;

namespace Sneal.SqlExporter.Core
{
    public class ExportParams : IExportParams
    {
        private bool scriptDataAsSql;
        private bool scriptDataAsXml;
        private bool scriptTableConstraints;
        private bool scriptTableIndexes;
        private bool scriptTableSchema;
        private List<string> sprocsToScript = new List<string>();
        private List<string> tablesToScript = new List<string>();
        private IList<string> tablesToScriptData = new List<string>();
        private bool useMultipleFiles;
        private List<string> viewsToScript = new List<string>();

        #region IExportParams Members

        public bool ScriptTableSchema
        {
            get { return scriptTableSchema; }
            set { scriptTableSchema = value; }
        }

        public bool ScriptTableConstraints
        {
            get { return scriptTableConstraints; }
            set { scriptTableConstraints = value; }
        }

        public bool ScriptTableIndexes
        {
            get { return scriptTableIndexes; }
            set { scriptTableIndexes = value; }
        }


        public bool ScriptDataAsSql
        {
            get { return scriptDataAsSql; }
            set { scriptDataAsSql = value; }
        }

        public bool ScriptDataAsXml
        {
            get { return scriptDataAsXml; }
            set { scriptDataAsXml = value; }
        }

        public IList<string> TablesToScriptData
        {
            get { return tablesToScriptData; }
            set { tablesToScriptData = value; }
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