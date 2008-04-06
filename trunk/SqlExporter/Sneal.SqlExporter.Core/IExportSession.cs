using System;
using System.Collections.Generic;
using SqlAdmin;

namespace Sneal.SqlExporter.Core
{
    public interface IExportSession : IDisposable
    {
        SqlDatabase Database { get; }

        /// <summary>
        /// This event fires each time a script is written to disk.
        /// </summary>
        event EventHandler<ProgressEventArgs> ProgressEvent;

        IList<string> GetUserTables();
        IList<string> GetUserSprocs();
        IList<string> GetUserViews();

        void Export(string exportDirectory, IExportParams exportParams);
    }
}