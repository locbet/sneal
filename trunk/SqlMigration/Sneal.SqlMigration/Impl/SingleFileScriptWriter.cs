using System;
using System.IO;
using Sneal.Preconditions;

namespace Sneal.SqlMigration.Impl
{
    public class SingleFileScriptWriter : IScriptWriter
    {
        private string exportDirectory;
        private readonly string scriptName;
        private IScriptMessageManager messageManager;
        private bool append;

        public SingleFileScriptWriter(string exportDirectory, string scriptName)
        {
            Throw.If(exportDirectory).IsEmpty();
            Throw.If(scriptName).IsEmpty();

            if (!scriptName.EndsWith(".sql", StringComparison.OrdinalIgnoreCase))
                scriptName += ".sql";

            this.exportDirectory = exportDirectory;
            this.scriptName = scriptName;
        }

        #region IScriptWriter Members

        public virtual string ExportDirectory
        {
            get { return exportDirectory; }
            set { exportDirectory = value; }
        }

        public IScriptMessageManager MessageManager
        {
            get { return messageManager; }
            set { messageManager = value; }
        }

        public virtual void WriteIndexScript(string objectName, string sql)
        {
            WriteScript(objectName, sql);
        }

        public virtual void WriteTableScript(string objectName, string sql)
        {
            WriteScript(objectName, sql);
        }

        public virtual void WriteViewScript(string objectName, string sql)
        {
            WriteScript(objectName, sql);
        }

        public virtual void WriteSprocScript(string objectName, string sql)
        {
            WriteScript(objectName, sql);
        }

        public virtual void WriteTableDataScript(string objectName, string sql)
        {
            WriteScript(objectName, sql);
        }

        public void WriteForeignKeyScript(string objectName, string sql)
        {
            WriteScript(objectName, sql);
        }

        #endregion

        protected virtual void WriteScript(string objectName, string sql)
        {
            Throw.If(objectName).IsEmpty();

            if (sql != null)
                sql = sql.Trim();

            if (string.IsNullOrEmpty(sql))
            {
                messageManager.OnScriptMessage(string.Format("{0} is empty.", objectName));
            }
            else
            {
                string scriptPath = Path.Combine(exportDirectory, scriptName);

                try
                {
                    if (!Directory.Exists(exportDirectory))
                        Directory.CreateDirectory(exportDirectory);

                    if (File.Exists(scriptPath))
                        File.SetAttributes(scriptPath, FileAttributes.Normal);

                    using (TextWriter scriptFile = new StreamWriter(scriptPath, append))
                    {
                        scriptFile.WriteLine("");
                        scriptFile.WriteLine(sql);
                    }

                    append = true;
                }
                catch (Exception ex)
                {
                    string msg = string.Format("Could not write the script file {0} to disk.",
                                               scriptPath);
                    throw new SqlMigrationException(msg, ex);
                }

                messageManager.OnScriptMessage(scriptPath);
            }
        }
    }
}