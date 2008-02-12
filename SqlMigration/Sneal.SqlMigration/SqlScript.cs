using System.Collections.Specialized;
using System.Text;

namespace Sneal.SqlMigration
{
    public class SqlScript
    {
        private readonly StringBuilder script = new StringBuilder();

        public SqlScript() {}

        public SqlScript(string line)
        {
            Append(line);
        }

        public SqlScript Append(SqlScript scriptToAppend)
        {
            script.Append(scriptToAppend.script);
            return this;
        }

        public SqlScript Append(StringBuilder scriptToAppend)
        {
            script.Append(scriptToAppend.ToString());
            return this;
        }

        public SqlScript Append(StringCollection scriptToAppend)
        {
            foreach (string line in scriptToAppend)
                script.Append(line);

            return this;
        }

        public SqlScript Append(string scriptToAppend)
        {
            script.Append(scriptToAppend);
            return this;
        }

        public SqlScript AppendFormat(string format, params object[] parms)
        {
            script.AppendFormat(format, parms);
            return this;
        }

        public static SqlScript operator+(SqlScript line1, SqlScript line2)
        {
            return line1.Append(line2);
        }

        public static implicit operator SqlScript(string line)
        {
            return new SqlScript(line);
        }

        public string ToScript()
        {
            return script.ToString();
        }

        public int Length
        {
            get { return script.Length; }
        }
    }
}
