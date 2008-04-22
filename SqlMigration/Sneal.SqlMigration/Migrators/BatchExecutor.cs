using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;
using MyMeta;
using Sneal.Preconditions;
using Sneal.SqlMigration.Impl;
using Sneal.SqlMigration.IO;

namespace Sneal.SqlMigration.Migrators
{
    /// <summary>
    /// Executes a SQL script as separate batches.  Command batches are
    /// separated using the batch delimeter keyword.
    /// </summary>
    public class BatchExecutor : IExecutor
    {
        protected IDatabase currentDb;
        protected IScriptFile currentScript;
        protected IFileSystem fileSystem = new FileSystemAdapter();
        protected IScriptMessageManager messageManager;
        public string BatchDelimiter;
        public int BatchTimeoutInSeconds = 120;

        public BatchExecutor()
            : this(null) { }

        public BatchExecutor(string batchDelimiter)
            : this(batchDelimiter, new NullScriptMessageManager()) { }

        public BatchExecutor(string batchDelimiter, IScriptMessageManager messageManager)
        {
            Throw.If(messageManager, "MessageManager").IsNull();
            BatchDelimiter = batchDelimiter;
            this.messageManager = messageManager;
        }

        public virtual IScriptMessageManager MessageManager
        {
            get { return messageManager; }
            set
            {
                Throw.If(value, "MessageManager").IsNull();
                messageManager = value;
            }
        }

        public virtual IFileSystem FileSystem
        {
            get { return fileSystem; }
            set
            {
                Throw.If(value, "FileSystem").IsNull();
                fileSystem = value;
            }
        }

        #region IExecutor Members

        /// <summary>
        /// Executes the specified SQL script in batches.
        /// </summary>
        /// <param name="db">The database instance to run the script against.</param>
        /// <param name="sqlFile">The T-SQL script to execute.</param>
        public virtual void Execute(IDatabase db, IScriptFile sqlFile)
        {
            Throw.If(db, "db").IsNull();
            Throw.If(!sqlFile.IsSql, "sqlFile");

            if (!fileSystem.Exists(sqlFile.Path))
            {
                throw new SqlMigrationException(
                    string.Format(
                        "Could not find the SQL script file {0}",
                        sqlFile.Path));
            }

            string[] sqlBatches;

            if (!string.IsNullOrEmpty(BatchDelimiter))
            {
                Regex regex = new Regex("^" + BatchDelimiter, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                sqlBatches = regex.Split(fileSystem.ReadToEnd(sqlFile.Path));
            }
            else
            {
                sqlBatches = new string[1];
                sqlBatches[0] = fileSystem.ReadToEnd(sqlFile.Path);
            }

            currentScript = sqlFile;
            currentDb = db;

            ExecuteSqlInternal(sqlBatches);
        }

        #endregion

        /// <summary>
        /// Executes each batch as a separate command.
        /// </summary>
        protected virtual void ExecuteSqlInternal(IEnumerable<string> batches)
        {
            using (IDbConnection connection = DatabaseConnectionFactory.CreateDbConnection(currentDb))
            {
                IDbCommand cmd = connection.CreateCommand();

                foreach (string batch in batches)
                {
                    if (batch == null || batch.Trim().Length == 0)
                        continue;

                    cmd.CommandText = batch;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = BatchTimeoutInSeconds;

                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (DbException ex)
                    {
                        string msg = string.Format(
                            "An error occurred while trying to run the SQL script {0} against database {1}",
                            currentScript.Path, currentDb.Name);
                        throw new SqlMigrationException(msg, ex);
                    }
                }
            }
        }
    }
}