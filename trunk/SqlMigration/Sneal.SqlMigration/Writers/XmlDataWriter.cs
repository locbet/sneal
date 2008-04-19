using System;
using System.IO;
using Sneal.Preconditions;
using Sneal.SqlMigration.Impl;

namespace Sneal.SqlMigration.Writers
{
    /// <summary>
    /// IScriptWriter that can only be used to write table data as individual
    /// XML files.
    /// </summary>
    /// <remarks>
    /// Only the WriteTableDataScript method is supported, all other methods
    /// will throw a NotSupportedException.
    /// </remarks>
    public class XmlDataWriter : IScriptWriter
    {
        private string exportDirectory;
        private IScriptMessageManager messageManager;

        public XmlDataWriter(string exportDirectory, IScriptMessageManager messageManager)
        {
            Throw.If(exportDirectory, "exportDirectory").IsEmpty();
            Throw.If(messageManager, "messageManager").IsNull();

            this.exportDirectory = exportDirectory;
            this.messageManager = messageManager;
        }

        public XmlDataWriter(string exportDirectory)
        {
            Throw.If(exportDirectory, "exportDirectory").IsEmpty();

            this.exportDirectory = exportDirectory;
            messageManager = new NullScriptMessageManager();
        }

        #region IScriptWriter Members

        public virtual string ExportDirectory
        {
            get { return exportDirectory; }
            set
            {
                Throw.If(value, "ExportDirectory").IsEmpty();
                exportDirectory = value;
            }
        }

        public virtual IScriptMessageManager MessageManager
        {
            get { return messageManager; }
            set
            {
                Throw.If(value, "MessageManager").IsNull();
                messageManager = value;
            }
        }

        public virtual void WriteIndexScript(string objectName, string sql)
        {
            throw new NotSupportedException("XmlDataWriter cannot write this script type.");
        }

        public virtual void WriteTableScript(string objectName, string sql)
        {
            throw new NotSupportedException("XmlDataWriter cannot write this script type.");
        }

        public virtual void WriteViewScript(string objectName, string sql)
        {
            throw new NotSupportedException("XmlDataWriter cannot write this script type.");
        }

        public virtual void WriteSprocScript(string objectName, string sql)
        {
            throw new NotSupportedException("XmlDataWriter cannot write this script type.");
        }

        public virtual void WriteTableDataScript(string objectName, string sql)
        {
            WriteXmlFile(objectName, sql);
        }

        public virtual void WriteForeignKeyScript(string objectName, string sql)
        {
            throw new NotSupportedException("XmlDataWriter cannot write this script type.");
        }

        #endregion

        protected virtual void WriteXmlFile(string objectName, string xml)
        {
            Throw.If(objectName).IsEmpty();

            if (xml != null)
                xml = xml.Trim();

            if (string.IsNullOrEmpty(xml))
            {
                messageManager.OnScriptMessage(string.Format("{0} is empty.", objectName));
            }
            else
            {
                string dir = Path.Combine(exportDirectory, "Data");
                string scriptPath = Path.Combine(dir, objectName + ".xml");

                try
                {
                    // TODO: Hide the direct disk access behind an interface
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    if (File.Exists(scriptPath))
                        File.SetAttributes(scriptPath, FileAttributes.Normal);

                    using (TextWriter scriptFile = new StreamWriter(scriptPath, false))
                    {
                        scriptFile.WriteLine(xml);
                    }
                }
                catch (Exception ex)
                {
                    string msg = string.Format("Could not write the xml file {0} to disk.",
                                               scriptPath);
                    throw new SqlMigrationException(msg, ex);
                }

                messageManager.OnScriptMessage(scriptPath);
            }
        }
    }
}