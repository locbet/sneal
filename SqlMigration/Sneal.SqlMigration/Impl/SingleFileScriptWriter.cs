using System;
using System.IO;
using Sneal.Preconditions;

namespace Sneal.SqlMigration.Impl
{
    public class SingleFileScriptWriter : IScriptWriter
    {
        private string exportDirectory;
        private IScriptMessageManager messageManager;

        public SingleFileScriptWriter(string exportDirectory)
        {
            this.exportDirectory = exportDirectory;
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

        public virtual void WriteConstraintScript(string objectName, string sql)
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

        #endregion

        protected virtual void WriteScript(string objectName, string sql)
        {
            Throw.If(objectName).IsEmpty();

            if (sql != null)
                sql = sql.Trim();

            // TODO: allow script name to be changed.
            string scriptPath = Path.Combine(exportDirectory, "database.sql");

            if (string.IsNullOrEmpty(sql))
            {
                messageManager.OnScriptMessage(string.Format("{0} is empty.", objectName));
            }
            else
            {
                try
                {
                    if (!Directory.Exists(exportDirectory))
                        Directory.CreateDirectory(exportDirectory);

                    if (File.Exists(scriptPath))
                        File.SetAttributes(scriptPath, FileAttributes.Normal);

                    using (StreamWriter scriptFile = new StreamWriter(scriptPath, true))
                    {
                        scriptFile.Write(sql);
                    }
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