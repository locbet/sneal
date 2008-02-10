using System.Collections;
using log4net.Config;
using NHibernate;
using NHibernate.Cfg;
using NUnit.Framework;
using Sneal.SqlMigration.Impl;

namespace Sneal.SqlMigration.Tests
{
    /// <summary>
    /// These tests require the freely downloadable AdventureWork OLTP DB.
    /// </summary>
    [TestFixture]
    public class MappingFixture
    {
        #region Setup/Teardown

        [TestFixtureSetUp]
        public void OneTimeSetUp()
        {
            XmlConfigurator.Configure();
            
            Hashtable props = new Hashtable();
            props.Add("hibernate.dialect", "NHibernate.Dialect.MsSql2000Dialect");
            props.Add("hibernate.connection.connection_string_name", "AdventureWorks");
            props.Add("hibernate.connection.driver_class", "NHibernate.Driver.SqlClientDriver");
            props.Add("hibernate.connection.provider", "NHibernate.Connection.DriverConnectionProvider");
            props.Add("show_sql", true);

            // Create NHibernate config
            Configuration configuration = new Configuration();
            configuration.SetProperties(props);
            configuration.AddAssembly("Sneal.SqlMigration");

            sessionFactory = configuration.BuildSessionFactory();
        }

        [SetUp]
        public void SetUp()
        {
            session = sessionFactory.OpenSession();
        }

        [TearDown]
        public void TearDown()
        {
            if (session != null && session.IsOpen)
                session.Close();
        }


        #endregion

        private ISession session;
        private ISessionFactory sessionFactory;

        [Test]
        public void ShouldReturnContactTable()
        {
            ITable table = session.Get<Table>("Contact");

            Assert.IsNotNull(table, "No table object returned");
            Assert.AreEqual("Contact", table.Name);
            Assert.AreEqual("Person", table.Schema);
        }

        [Test]
        public void ShouldReturnContactTableColumns()
        {
            ITable table = session.Get<Table>("Contact");

            Assert.IsNotNull(table, "No table object returned");
            Assert.IsNotNull(table.Columns, "No column objects returned");
            Assert.AreEqual(15, table.Columns.Count);
            Assert.AreEqual("ContactID", table.Columns[0].Name);
            Assert.AreEqual("NameStyle", table.Columns[1].Name);
            Assert.AreEqual(table, table.Columns[0].Table, "No back link to table");
        }

        [Test]
        public void ShouldReturnAddressForeignKeys()
        {
            ITable address = session.Get<Table>("Address");
            ITable state = session.Get<Table>("StateProvince");

            Assert.IsNotNull(address, "No address table returned");
            Assert.IsNotNull(state, "No state table returned");
            Assert.IsNotNull(address.Columns, "No address columns returned");

            Assert.AreEqual("FK_Address_StateProvince_StateProvinceID",
                address.Columns[4].ForeignKey.Name, "Incorrect FK name");

            Assert.AreEqual(address.Columns[4],
                address.Columns[4].ForeignKey.ForeignKeyColumn, "ForeignKeyColumn StateProvinceID is incorrect");

            Assert.AreEqual(state.Columns[0],
                address.Columns[4].ForeignKey.PrimaryKeyColumn, "PrimaryKeyColumn StateProvinceID is incorrect");
        }
    }
}