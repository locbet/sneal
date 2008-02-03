using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using Sneal.SqlMigration.Impl;

namespace Sneal.SqlMigration.Tests
{
    [TestFixture]
    public class SqlScriptWriterFixture
    {
        private MockRepository mocks;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
        }

        [Test]
        public void ShouldWriteColumnDefinition()
        {
            IColumn column = mocks.Stub<IColumn>();

            SetupResult.For(column.Name).Return("Name");
            SetupResult.For(column.IsNullable).Return(false);
            SetupResult.For(column.DataType).Return(SqlDataType.NVarChar);

            mocks.ReplayAll();

            SqlServerScriptHelper helper = new SqlServerScriptHelper();
            string script = helper.WriteColumn(column);

            Assert.AreEqual("[Name] [NVARCHAR] (50) NOT NULL", script);
        }
    }
}
	