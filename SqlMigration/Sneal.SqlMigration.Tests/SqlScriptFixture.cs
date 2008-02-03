using System.Text;
using NUnit.Framework;
using Sneal.SqlMigration;

namespace DefaultNamespace
{
    [TestFixture]
    public class SqlScriptFixture
    {
        [Test]
        public void ShouldAppendScriptFromStringBuilder()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("DROP TABLE [table1]");
            builder.AppendLine("DROP TABLE [table2]");

            SqlScript script = new SqlScript();
            script.Append(builder);

            Assert.AreEqual(builder.Length, script.Length, "Lengths are no equals");
            Assert.AreEqual("DROP TABLE [table1]\r\nDROP TABLE [table2]\r\n", script.ToScript(),
                            "Script contents don't matchs");
        }
    }
}