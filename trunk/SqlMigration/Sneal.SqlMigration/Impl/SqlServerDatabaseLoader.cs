using System;
using System.Collections.Generic;
using System.Text;
using Sneal.SqlMigration.Utils;

namespace Sneal.SqlMigration.Impl
{
    /// <summary>
    /// SQL Server specific strategy for loading a migration database.
    /// </summary>
    public class SqlServerDatabaseLoader : IDatabaseLoader
    {
        private Database db;
        private IDatabaseConnectionInfo connectionInfo;
        private IScriptMessageManager messageManager = new NullScriptMessageManager();

        public IDatabase LoadDatabase(IDatabaseConnectionInfo connection)
        {
            Should.NotBeNull(connection, "connection");
            connectionInfo = connection;

            messageManager.OnScriptMessage(string.Format("Starting to load database {0} into memory.", connection.Name));

            db = new Database();

            LoadTables();
            LoadViews();
            LoadSprocs();

            messageManager.OnScriptMessage(string.Format("Finished loading {0}.", connection.Name));

            return db;
        }

        protected virtual void LoadTables()
        {
            // option 1
            // create a DAO class which returns IDataReader for each query
            // data will come from information_schema views and system tables

            // option 2
            // connect directly to db using SQL Server 2005 SMO libraries

            // get list of all tables
            // iterate list of tables
            //   get all columns
            //   iterate all columns
            //      get all indexes for column
            //      get all fks for column
        }

        protected virtual void LoadViews()
        {
            //throw new NotImplementedException();
        }

        protected virtual void LoadSprocs()
        {
            //throw new NotImplementedException();
        }

        public IScriptMessageManager MessageManager
        {
            get { return messageManager; }
            set
            {
                Should.NotBeNull(value, "MessageManager");
                messageManager = value;
            }
        }
    }
}
