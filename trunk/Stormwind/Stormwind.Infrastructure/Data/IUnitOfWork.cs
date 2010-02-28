using System;

namespace Stormwind.Infrastructure.Data
{
    /// <summary>
    /// Represents a data access unit of work.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// The current commit mode, which defaults to explicit mode.
        /// </summary>
        CommitMode CommitMode { get; set; }

        /// <summary>
        /// Starts the unit of work, which creates a read committed transaction 
        /// for the duration of the unit of work.
        /// </summary>
        void Start();

        /// <summary>
        /// Makes the unit of work consistent with the underlying data storage
        /// within the current transaction. This does not commit the transaction.
        /// </summary>
        void Flush();

        /// <summary>
        /// Commits the unit of work with the underlying storage, thus making
        /// the changes permanent and available to other units of work.
        /// </summary>
        void Commit();
    }
}