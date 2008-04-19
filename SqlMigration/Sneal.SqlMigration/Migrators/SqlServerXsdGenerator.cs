using System.IO;
using System.Xml;
using System.Xml.Schema;
using MyMeta;
using Sneal.SqlMigration.Utils;

namespace Sneal.SqlMigration.Migrators
{
    /// <summary>
    /// Generates a SQLXML mapping schema from a table.
    /// </summary>
    public class SqlServerXsdGenerator : ITableXsdGenerator
    {
        /*
        Should generate an XSD that looks something like this:
        
        <xsd:schema xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:sql="urn:schemas-microsoft-com:mapping-schema">
            <xsd:element name="Customers">
                <xsd:complexType>
                    <xsd:sequence>
                        <xsd:element name="CustomerID" type="xsd:integer" sql:field="CustomerID" />
                        <xsd:element name="CompanyName" type="xsd:string" sql:field="CompanyName" />
                        <xsd:element name="City" type="xsd:string" sql:field="City" />
                    </xsd:sequence>
                </xsd:complexType>
            </xsd:element>
        </xsd:schema>
        */

        private const string MappingSchemaNS = "urn:schemas-microsoft-com:mapping-schema";
        private const string XmlDataTypeNS = "http://www.w3.org/2001/XMLSchema";

        #region ITableXsdGenerator Members

        /// <summary>
        /// Generates a SQL Server XML schema for the given table and XML data.
        /// </summary>
        /// <remarks>
        /// Its assumed that the XML elements (columns) have the same name as
        /// the table columns.
        /// </remarks>
        /// <param name="table">The table associated with the XML.</param>
        /// <returns>The XSD contents as a string.</returns>
        public virtual XmlSchema GenerateXsd(ITable table)
        {
            XmlSchema xsd = new XmlSchema();

            xsd.Namespaces.Add("sql", MappingSchemaNS);

            XmlSchemaElement tableElement = new XmlSchemaElement();
            tableElement.Name = table.Name;
            xsd.Items.Add(tableElement);

            XmlSchemaComplexType complexType = new XmlSchemaComplexType();
            tableElement.SchemaType = complexType;

            XmlSchemaSequence seq = new XmlSchemaSequence();
            complexType.Particle = seq;

            foreach (IColumn col in table.Columns)
            {
                string xmlDataType = DataTypeUtil.ToXmlDataType(col);

                XmlSchemaElement colElem = new XmlSchemaElement();
                colElem.Name = col.Name;
                colElem.MaxOccurs = 1;
                colElem.MinOccurs = 1;
                colElem.SchemaTypeName = new XmlQualifiedName(xmlDataType, XmlDataTypeNS);

                XmlDocument xml = new XmlDocument();
                XmlAttribute attr = xml.CreateAttribute("sql", "field", MappingSchemaNS);
                attr.Value = col.Name;

                XmlAttribute[] sqlAttrs = {attr};
                colElem.UnhandledAttributes = sqlAttrs;

                seq.Items.Add(colElem);
            }

            return xsd;

//            StringWriter w = new StringWriter();
//            xsd.Write(w);
//
//            return w.ToString();
        }

        #endregion
    }
}