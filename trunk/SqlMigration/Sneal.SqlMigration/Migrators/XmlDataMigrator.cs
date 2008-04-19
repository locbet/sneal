using System.Data;
using System.Globalization;
using System.Text;
using MyMeta;
using Sneal.Preconditions;

namespace Sneal.SqlMigration.Migrators
{
    /// <summary>
    /// Turns table data into a SQL Server compatable bulk load XML document.
    /// </summary>
    public class XmlDataMigrator : DataMigratorBase, IDataMigrator
    {
        // TODO: Generate the XSD at bulk load time.
        // TODO: Refactor data migrators ->
        // All 3 of these data migrators need to be refactored.  Too much of the
        // shared functionality is in the base class.  The INSERT, UPDATE, DELETE
        // generation should be in one or more scripting strategies.  We could
        // have a generation strategy for each DB type, or another for XML.

        /// <summary>
        /// Scripts all of the data in a table as XML elements.
        /// </summary>
        /// <remarks>
        /// Although this method takes a SqlScript parameter it should really take
        /// and return some other object type, like an XmlDocument or string.
        /// Using a SqlScript is really just a hack, and will not work as intended
        /// when UseMultipleFiles is set to false.
        /// </remarks>
        public virtual SqlScript ScriptAllData(ITable sourceTable, SqlScript script)
        {
            Throw.If(sourceTable, "sourceTable").IsNull();
            Throw.If(script, "script").IsNull();

            Source = sourceTable;

            DataTable sourceDataTable = GetTableData(Source);
            SqlScript dataScript = new SqlScript("<ROOT>\r\n");

            foreach (DataRow sourceRow in sourceDataTable.Rows)
            {
                dataScript += string.Format("\t<{0}>\r\n", sourceTable.Name);
                dataScript += ScriptInsertRow(sourceRow);
                dataScript += string.Format("\t</{0}>\r\n", sourceTable.Name);
            }
            
            dataScript += "</ROOT>\r\n";

            script += dataScript;
            return script;
        }

        protected override string ScriptInsertRow(DataRow row)
        {
            Throw.If(row, "row").IsNull();

            StringBuilder values = new StringBuilder();
            foreach (IColumn col in Source.Columns)
            {
                if (col.IsComputed)
                    continue;

                string colVal = GetXmlColumnValue(col, row);

                values.AppendFormat(CultureInfo.InvariantCulture, "\t\t<{0}>", col.Name);
                values.Append(colVal);
                values.AppendFormat(CultureInfo.InvariantCulture, "</{0}>\r\n", col.Name);
            }

            return values.ToString();
        }

        private static string GetXmlColumnValue(IColumn col, DataRow row)
        {
            string val = GetColumnValue(col, row);
            if (val.StartsWith("'") && val.EndsWith("'"))
                val = val.Substring(1, val.Length - 2);

            return val;
        }
    }
}