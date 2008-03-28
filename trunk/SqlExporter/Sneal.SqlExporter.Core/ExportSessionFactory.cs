using System;
using System.Collections.Generic;
using Sneal.Preconditions;
using SqlAdmin;

namespace Sneal.SqlExporter.Core
{
    public class ExportSessionFactory
    {
        private readonly IConnectionSettings settings;
        private SqlServerConnection connection;

        public ExportSessionFactory(IConnectionSettings settings)
        {
            Throw.If(settings).IsNull();
            Throw.If(settings.ServerName).IsEmpty();

            this.settings = settings;

            CreateConnection();
        }

        protected void CreateConnection()
        {
            try
            {
                if (settings.UseIntegratedAuthentication)
                    connection = new SqlServerConnection(settings.ServerName);
                else
                    connection = new SqlServerConnection(settings.ServerName, settings.UserName, settings.Password);

                connection.Connect();

                if (!connection.IsUserValid())
                {
                    string msg = string.Format(
                        "The user is not valid for SQL Server {0}",
                        settings.ServerName);
                    throw new SqlExporterConnectionException(msg);                    
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format(
                    "There was an error while trying to connect to the SQL Server {0}",
                    settings.ServerName);
                throw new SqlExporterConnectionException(msg, ex);
            }            
        }

        public virtual IExportSession CreateExportSession(string database)
        {
            Throw.If(database).IsEmpty();

            try
            {
                IExportSession exportSession = new ExportSession(connection, database);
                return exportSession;
            }
            catch (Exception ex)
            {
                string msg = string.Format(
                    "There was an error while trying to connect to the database {1} on server {0}",
                    database, settings.ServerName);
                throw new SqlExporterConnectionException(msg, ex);
            }
        }

        public IList<string> GetDatabaseNames()
        {
            List<string> databases = new List<string>();

            try
            {
                foreach (SqlDatabase db in connection.Databases)
                {
                    databases.Add(db.Name);
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format(
                    "There was an error while trying to connect to enumerate the databases on server {0}",
                    settings.ServerName);
                throw new SqlExporterException(msg, ex);                
            }

            return databases;
        }
    }
}