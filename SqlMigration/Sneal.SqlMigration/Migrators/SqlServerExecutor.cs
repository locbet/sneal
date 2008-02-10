using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using Sneal.SqlMigration.Impl;

namespace Sneal.SqlMigration.Migrators
{
    public class SqlServerExecutor
    {
        private readonly SqlConnection dbConnection;
        private readonly IScriptMessageManager messageManager = new NullScriptMessageManager();
        public string Delimeter = "GO";
        private string[] sqlBatches;
        public int Timeout = 120;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerExecutor"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public SqlServerExecutor(SqlConnection connection)
        {
            dbConnection = connection;
        }

        /// <summary>
        /// Executes the specified SQL in batches.
        /// </summary>
        /// <param name="sql">The raw SQL statements to execute.</param>
        /// <returns>Returns <c>true</c> if no errors occurred, otherwise <c>false</c>.</returns>
        public bool Execute(string sql)
        {
            Regex regex = new Regex("^" + Delimeter, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            sqlBatches = regex.Split(sql);

            return ExecuteSqlInternal();
        }

        /// <summary>
        /// Executes the SQL statements.
        /// </summary>
        private bool ExecuteSqlInternal()
        {
            bool success = true;
            using (SqlCommand cmd = dbConnection.CreateCommand())
            {
                cmd.Connection = dbConnection;

                foreach (string line in sqlBatches)
                {
                    if (line.Trim().Length == 0)
                        continue;

                    cmd.CommandText = line;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = Timeout;

                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        success = false;
                        messageManager.OnScriptMessage(ex.Message);
                    }
                }
            }

            return success;
        }
    }
}