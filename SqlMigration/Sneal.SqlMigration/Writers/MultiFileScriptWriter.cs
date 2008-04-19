using System;
using System.IO;
using Sneal.Preconditions;
using Sneal.SqlMigration.Impl;

namespace Sneal.SqlMigration.Writers
{
    public class MultiFileScriptWriter : IScriptWriter
    {
        private string exportDirectory;
        private IScriptMessageManager messageManager = new NullScriptMessageManager();

        public MultiFileScriptWriter(string exportDirectory)
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
            WriteScript(objectName, "Schema\\Indexes", sql);
        }

        public virtual void WriteTableScript(string objectName, string sql)
        {
            WriteScript(objectName, "Schema\\Tables", sql);
        }

        public void WriteForeignKeyScript(string objectName, string sql)
        {
            WriteScript(objectName, "Schema\\ForeignKeys", sql);
        }

        public virtual void WriteViewScript(string objectName, string sql)
        {
            WriteScript(objectName, "Views", sql);
        }

        public virtual void WriteSprocScript(string objectName, string sql)
        {
            WriteScript(objectName, "Sprocs", sql);
        }

        public virtual void WriteTableDataScript(string objectName, string sql)
        {
            WriteScript(objectName, "Data", sql);
        }

        #endregion

        protected virtual void WriteScript(string objectName, string subFolderName, string sql)
        {
            Throw.If(objectName).IsEmpty();

            if (sql != null)
                sql = sql.Trim();

            // write sproc to file
            string dir = Path.Combine(exportDirectory, subFolderName);
            string scriptPath = Path.Combine(dir, objectName + ".sql");

            if (string.IsNullOrEmpty(sql))
            {
                messageManager.OnScriptMessage(string.Format("{0} is empty.", objectName));
            }
            else
            {
                try
                {
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    if (File.Exists(scriptPath))
                        File.SetAttributes(scriptPath, FileAttributes.Normal);

                    using (StreamWriter scriptFile = new StreamWriter(scriptPath, false))
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