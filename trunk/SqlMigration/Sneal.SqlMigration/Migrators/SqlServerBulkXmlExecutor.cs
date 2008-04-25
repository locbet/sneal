using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml.Schema;
using System.Xml.XPath;
using MyMeta;
using Sneal.Preconditions;
using Sneal.SqlMigration.IO;
using Sneal.SqlMigration.Utils;

namespace Sneal.SqlMigration.Migrators
{
    /// <summary>
    /// Bulk loads the given XML document into the specified SQL Server's
    /// table.  This makes use of the XML components of SQL Server 2000
    /// or SQL Server 2005 (SQLXML).  SQL Server 2000 doesn't come
    /// come preinstalled with SQLXML, a separate download and install
    /// is required.
    /// </summary>
    /// <remarks>
    /// If you try to bulk load a table that has a computed column that is
    /// dependant upon the identity value you will have problems, see:
    /// <see cref="http://support.microsoft.com/kb/330582/"/>
    /// </remarks>
    public class SqlServerBulkXmlExecutor : IExecutor
    {
        private IFileSystem fileSystem = new FileSystemAdapter();
        private ITableXsdGenerator schemaGenerator = new SqlServerXsdGenerator();

        public virtual IFileSystem FileSystem
        {
            get { return fileSystem; }
            set
            {
                Throw.If(value, "FileSystem").IsNull();
                fileSystem = value;
            }
        }

        public virtual ITableXsdGenerator SchemaGenerator
        {
            get { return schemaGenerator; }
            set
            {
                Throw.If(value, "SchemaGenerator").IsNull();
                schemaGenerator = value;
            }
        }

        #region IExecutor Members

        public virtual void Execute(IDatabase db, IScriptFile xmlFile)
        {
            Throw.If(db, "db").IsNull();
            Throw.If(xmlFile, "xmlFile").IsNull();
            Throw.If(!xmlFile.IsXml, "xmlFile");

            DbObjectName tableName = GetTableNameFromXmlElement(xmlFile);
            ITable table = MyMetaUtil.GetTableOrThrow(db, tableName);

            LoadTable(table, xmlFile.Path);
        }

        #endregion

        /// <summary>
        /// Loads the table using the specified xml data file.
        /// </summary>
        /// <param name="table">The reference to the table to load.</param>
        /// <param name="xmlDataPath">The path to the XML doc.</param>
        public virtual void LoadTable(ITable table, string xmlDataPath)
        {
            Throw.If(table).IsNull();
            Throw.If(!fileSystem.Exists(xmlDataPath));

            string schameFilePath = CreateTemporaryXsdFileForTable(table);
            ExecuteSqlXmlBulkLoader(table, xmlDataPath, schameFilePath);
        }

        /// <summary>
        /// Gets the table name which should match the main XML element which
        /// is the child of ROOT.
        /// </summary>
        /// <param name="xmlFile">The xml data file path.</param>
        /// <returns>The table name, without the schema name.</returns>
        public virtual DbObjectName GetTableNameFromXmlElement(IScriptFile xmlFile)
        {
            Debug.Assert(xmlFile != null);

            if (!fileSystem.Exists(xmlFile.Path))
            {
                throw new SqlMigrationException(
                    string.Format(
                        "Could not find the XML data file {0}",
                        xmlFile.Path));
            }

            using (Stream content = xmlFile.GetContentStream())
            {
                XPathDocument doc = new XPathDocument(content);
                XPathNavigator navi = doc.CreateNavigator();

                navi.MoveToRoot();
                if (!navi.MoveToFirstChild()) // ROOT
                {
                    throw new SqlMigrationException(
                        "XML document is empty.  Could not find ROOT element.");
                }
                if (!navi.MoveToFirstChild()) // Table
                {
                    throw new SqlMigrationException(
                        "XML document contains no table data.  Could not find table element.");
                }

                string elem = navi.LocalName;
                elem = elem.Replace("<", "").Replace(">", "").Trim();
                return elem;
            }
        }

        /// <summary>
        /// Runs the SQLXMLBulkLoad COM component using the supplied arguments.
        /// </summary>
        /// <param name="table">The table to load.</param>
        /// <param name="xmlDataPath">Path to the XML data doc.</param>
        /// <param name="schemaPath">Path to the XSD table schema.</param>
        protected virtual void ExecuteSqlXmlBulkLoader(ITable table, string xmlDataPath, string schemaPath)
        {
            Throw.If(table, "table").IsNull();
            Throw.If(xmlDataPath, "xmlDataPath").IsEmpty();
            Throw.If(schemaPath, "schemaPath").IsEmpty();

            // Use late binding to create SQLXML bulk load COM instance
            // This avoids being bound to a specific version of the
            // component, and also avoids a direct project reference
            // since this portion of SqlMigration is optional.
            Type bulkLoaderType;
            object bulkLoader;

            try
            {
                bulkLoaderType = Type.GetTypeFromProgID("SQLXMLBulkLoad.SQLXMLBulkLoad");
            }
            catch (COMException ex)
            {
                throw new SqlMigrationException(
                    "Could not find the ProgID for SQLXMLBulkLoad.SQLXMLBulkLoad.  Is SQLXML installed?",
                    ex);
            }

            try
            {
                bulkLoader = Activator.CreateInstance(bulkLoaderType);
            }
            catch (COMException ex)
            {
                throw new SqlMigrationException(
                    "Could not create a COM interop instance of SQLXMLBulkLoad", ex);
            }

            // method parameters are passed in as an object array.
            string connStr = GetConnectionString(table);
            object[] oConnStr = new object[] {connStr};
            object[] oParms = new object[] {schemaPath, xmlDataPath};

            // Invoke the method the bulk loader's execute method
            try
            {
                bulkLoaderType.InvokeMember("ConnectionString", BindingFlags.SetProperty, null, bulkLoader, oConnStr);
                bulkLoaderType.InvokeMember("Execute", BindingFlags.InvokeMethod, null, bulkLoader, oParms);
            }
            catch (Exception ex)
            {
                string msg = string.Format(
                    "SQLXMLBulkLoad component could not load data from XML file {0} for table {1}",
                    xmlDataPath, table.Name);
                throw new SqlMigrationException(msg, ex);
            }
        }

        /// <summary>
        /// Runs the XSD generator and saves the output to a temp file for use
        /// in running the SQLXMLBulkLoad component.
        /// </summary>
        /// <param name="table">The table to generate an XSD from.</param>
        /// <returns>The path to the written XSD file.</returns>
        protected virtual string CreateTemporaryXsdFileForTable(ITable table)
        {
            XmlSchema schema = schemaGenerator.GenerateXsd(table);

            string schemaFilePath = fileSystem.GetTempFileName();
            using (TextWriter writer = fileSystem.OpenFileForWriting(schemaFilePath))
            {
                schema.Write(writer);
            }

            return schemaFilePath;
        }

        /// <summary>
        /// Gets the connection string that the table is using.
        /// </summary>
        private static string GetConnectionString(ITable table)
        {
            return table.Database.Root.ConnectionString;
        }
    }
}