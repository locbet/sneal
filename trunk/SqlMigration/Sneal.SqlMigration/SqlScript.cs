using System.Collections.Specialized;
using System.Text;

namespace Sneal.SqlMigration
{
    public class SqlScript
    {
        private readonly StringBuilder script = new StringBuilder();

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
