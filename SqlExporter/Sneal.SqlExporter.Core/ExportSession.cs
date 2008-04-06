using System;
using System.Collections.Generic;
using Sneal.Preconditions;
using SqlAdmin;

namespace Sneal.SqlExporter.Core
{
    public class ExportSession : IExportSession
    {
        private readonly SqlServerConnection connection;
        private readonly SqlDatabase database;
        private bool disposed;

        public ExportSession(SqlServerConnection connection, string databaseName)
        {
            Throw.If(connection).IsNull();
            Throw.If(databaseName).IsEmpty();

            this.connection = connection;

            try
            {
                database = connection.Databases[databaseName];
            }
            catch (Exception ex)
            {
                throw new SqlExporterConnectionException(
                    string.Format("Could not find or connect to the database {0} on server {1}",
                    databaseName, connection.Name), ex);                
            }

            if (database == null)
            {
                throw new SqlExporterConnectionException(
                    string.Format("Could not find or connect to the database {0} on server {1}",
                    databaseName, connection.Name));
            }
        }

        public SqlDatabase Database
        {
            get { return database; }
        }

        /// <summary>
        /// This event fires each time a script is written to disk.
        /// </summary>
        public event EventHandler<ProgressEventArgs> ProgressEvent;

        public IList<string> GetUserTables()
        {
            List<string> tables = new List<string>();
            foreach (SqlTable table in database.Tables)
            {
                if (table.TableType == SqlObjectType.User)
                    tables.Add(table.Name);
            }

            return tables;
        }

        public IList<string> GetUserSprocs()
        {
            List<string> sprocs = new List<string>();
            foreach (SqlStoredProcedure sproc in database.StoredProcedures)
            {
                if (sproc.StoredProcedureType == SqlObjectType.User && !sproc.Name.StartsWith("dt"))
                    sprocs.Add(sproc.Name);
            }

            return sprocs;
        }

        public IList<string> GetUserViews()
        {
            List<string> views = new List<string>();
            foreach (SqlView view in database.Views)
            {
                if (!view.Name.StartsWith("sys"))
                {
                    views.Add(view.Name);
                }
            }

            return views;
        }

        public void Export(string exportDirectory, IExportParams exportParams)
        {
            ScriptEngine engine = null;
            try
            {
                engine = new ScriptEngine(database);
                engine.ProgressEvent += Engine_ProgressEvent;
                engine.ExportScripts(exportParams, new MultiFileScriptWriter(exportDirectory));
            }
            finally
            {
                if (engine != null)
                    engine.ProgressEvent -= Engine_ProgressEvent;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void OnProgressEvent(ProgressEventArgs e)
        {
            EventHandler<ProgressEventArgs> evt = ProgressEvent;
            if (evt != null)
            {
                evt(this, e);
            }
        }

        private void Engine_ProgressEvent(object sender, ProgressEventArgs e)
        {
            // forward the event from the engine to session listeners
            OnProgressEvent(e);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                if (connection != null)
                    connection.Dispose();
            }

            disposed = true;
        }
    }
}