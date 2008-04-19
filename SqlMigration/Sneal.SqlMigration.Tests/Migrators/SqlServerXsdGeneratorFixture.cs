using System;
using System.Data.OleDb;
using System.IO;
using System.Text;
using System.Xml.Schema;
using NUnit.Framework;
using Sneal.SqlMigration.Migrators;
using Sneal.SqlMigration.Tests.TestHelpers;

namespace Sneal.SqlMigration.Tests.Migrators
{
    [TestFixture]
    public class SqlServerXsdGeneratorFixture
    {
        private const string ExpectedXsd =
            "<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
            "<xs:schema xmlns:sql=\"urn:schemas-microsoft-com:mapping-schema\" xmlns:xs=\"http://www.w3.org/2001/XMLSchema\">\r\n" +
            "  <xs:element name=\"Customers\">\r\n" +
            "    <xs:complexType>\r\n" +
            "      <xs:sequence>\r\n" +
            "        <xs:element sql:field=\"CustomerID\" minOccurs=\"1\" maxOccurs=\"1\" name=\"CustomerID\" type=\"xs:int\" />\r\n" +
            "        <xs:element sql:field=\"FirstName\" minOccurs=\"1\" maxOccurs=\"1\" name=\"FirstName\" type=\"xs:string\" />\r\n" +
            "        <xs:element sql:field=\"DateOfBirth\" minOccurs=\"1\" maxOccurs=\"1\" name=\"DateOfBirth\" type=\"xs:dateTime\" />\r\n" +
            "      </xs:sequence>\r\n" +
            "    </xs:complexType>\r\n" +
            "  </xs:element>\r\n" +
            "</xs:schema>";

        private SqlServerXsdGenerator gen;
        private TableStub table;

        [SetUp]
        public void SetUp()
        {
            table = new TableStub(new DatabaseStub("Test"), "Customers");
            ColumnStub col1 = table.AddStubbedColumn("CustomerID", "INT");
            col1.DataType = (int) OleDbType.Integer;
            ColumnStub col2 = table.AddStubbedColumn("FirstName", "NVARCHAR(50)");
            col2.DataType = (int) OleDbType.VarWChar;
            ColumnStub col3 = table.AddStubbedColumn("DateOfBirth", "DATETIME");
            col3.DataType = (int) OleDbType.Date;

            gen = new SqlServerXsdGenerator();
        }

        [Test]
        public void ShouldGenerateXsd()
        {
            XmlSchema xsd = gen.GenerateXsd(table);
            Assert.IsNotNull(xsd);

            StringWriter w = new StringWriter();
            xsd.Write(w);            
            Assert.AreEqual(ExpectedXsd, w.ToString());
        }

        [Test]
        public void ShouldGenerateXsdAndSaveToDisk()
        {
            XmlSchema xsd = gen.GenerateXsd(table);
            Assert.IsNotNull(xsd);
            
            using (StreamWriter sw = new StreamWriter(@"c:\temp\customer.xsd", false, Encoding.UTF8))
            {
                xsd.Write(sw);
            }

            using (StreamReader reader = new StreamReader(@"c:\temp\customer.xsd"))
            {
                Console.WriteLine(reader.ReadToEnd());
            }
        }
    }
}