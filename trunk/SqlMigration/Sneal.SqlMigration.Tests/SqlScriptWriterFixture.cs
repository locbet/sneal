using MyMeta;
using NUnit.Framework;
using Rhino.Mocks;
using Sneal.SqlMigration.Impl;

namespace Sneal.SqlMigration.Tests
{
    [TestFixture]
    public class SqlScriptWriterFixture
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
        }

        #endregion

        private MockRepository mocks;

        [Test]
        public void ShouldWriteColumnDefinition()
        {
            IColumn column = mocks.Stub<IColumn>();

            SetupResult.For(column.Name).Return("Name");
            SetupResult.For(column.IsNullable).Return(false);
            SetupResult.For(column.DataTypeNameComplete).Return("NVARCHAR(50)");

            mocks.ReplayAll();

            SqlServerScriptHelper helper = new SqlServerScriptHelper();
            string script = helper.WriteColumn(column);

            Assert.AreEqual("[Name] [NVARCHAR] (50) NOT NULL", script);
        }
    }
}