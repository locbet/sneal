using System;
using NUnit.Framework;

namespace Sneal.SqlMigration.Tests
{
    [TestFixture]
    public class DbObjectNameFixture
    {
        [Test]
        public void ShouldReturnOnlyObjectName()
        {
            string strName = "Customer";
            DbObjectName name = strName;

            Assert.AreEqual("", name.Schema);
            Assert.AreEqual("Customer", name.ShortName);
            Assert.AreEqual("Customer", name.ToString());
        }

        [Test]
        public void ShouldReturnSchemaAndObjectNameEvenForFullyQualifiedNames()
        {
            string strName = "localhost.Northwind.dbo.Customer";
            DbObjectName name = strName;

            Assert.AreEqual("dbo", name.Schema);
            Assert.AreEqual("Customer", name.ShortName);
            Assert.AreEqual("localhost.Northwind.dbo.Customer", name.ToString());
        }

        [Test]
        public void ShouldReturnSchemaAndObjectName()
        {
            string strName = "dbo.Customer";
            DbObjectName name = strName;

            Assert.AreEqual("dbo", name.Schema);
            Assert.AreEqual("Customer", name.ShortName);
            Assert.AreEqual("dbo.Customer", name.ToString());
        }

        [Test]
        public void ShouldCompareEqualityByValue()
        {
            DbObjectName name1 = "Customer";
            DbObjectName name2 = "Customer";

            Assert.AreEqual(name1, name2);
        }

        [Test]
        [ExpectedException(typeof (ArgumentException))]
        public void ShouldThrowWhenEmpty()
        {
            DbObjectName name = "";
        }
    }
}