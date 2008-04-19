using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml.Schema;
using MyMeta;
using Sneal.Preconditions;
using Sneal.SqlMigration.IO;

namespace Sneal.SqlMigration.Migrators
{
    /// <summary>
    /// Bulk loads the given XML document into the specified SQL Server's
    /// table.  This makes use of the XML components of SQL Server 2000
    /// or SQL Server 2005 (SQLXML).  SQL Server 2000 doesn't come
    /// come preinstalled with SQLXML, a separate download and install
    /// is required.
    /// </summary>
    public class SqlServerBulkXmlLoader
    {
        private IFileSystem fileSystem = new FileSystemAdapter();
        private ITableXsdGenerator schemaGenerator = new SqlServerXsdGenerator();

        public IFileSystem FileSystem
        {
            get { return fileSystem; }
            set { fileSystem = value; }
        }

        public ITableXsdGenerator SchemaGenerator
        {
            get { return schemaGenerator; }
            set { schemaGenerator = value; }
        }

        public virtual void LoadTable(ITable table, string xmlDataPath)
        {
            Throw.If(table).IsNull();
            Throw.If(!fileSystem.Exists(xmlDataPath));

            string schameFilePath = CreateTemporaryXsdFileForTable(table);
            ExecuteSqlXmlBulkLoader(table, xmlDataPath, schameFilePath);
        }

        protected virtual void ExecuteSqlXmlBulkLoader(ITable table, string xmlDataPath, string schemaPath)
        {
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
            object[] oConnStr = new object[] { connStr };
            object[] oParms = new object[] { schemaPath, xmlDataPath };

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

        private static string GetConnectionString(ITable table)
        {
            return table.Database.Root.ConnectionString;
        }

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
    }
}