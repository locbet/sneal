using System;
using System.Data;
using NHibernate;

namespace Stormwind.Infrastructure.Data
{
	/// <summary>
	/// Manages a unit of work around an NHibernate session, ensuring that a
	/// session is shared across all controllers, services, and entities
	/// during a request.
	/// <para>
	/// This class represents (currently), a one to one relation to a DB 
	/// transaction.
	/// </para>
	/// <para>
	/// Before using a UOW instance you must call Start(), which creates an
	/// open NHibernate session. A database transaction is started when Flush
	/// is first called or when Commit is called. If the commit mode is set to 
	/// implicit (the default), this UOW will commit any changes on Dispose().
	/// </para>
	/// </summary>
	public class UnitOfWork : IUnitOfWorkImplementor, IStartable
    {
    	private readonly ISessionProvider _sessionProvider;
        private ISession _session;
        private ITransaction _transaction;

        public UnitOfWork(ISessionProvider sessionProvider)
        {
        	_sessionProvider = sessionProvider;
        }

    	public virtual bool IsTransactionActive
        {
            get { return _transaction != null && _transaction.IsActive; }
        }

        public virtual ISession Session
        {
            get { return _session; }
        }

        public CommitMode CommitMode { get; set; }

        public virtual void Start()
        {
			if (_session == null)
			{
				_session = _sessionProvider.CreateSession();
			}
        }

        public virtual void Flush()
        {
        	EnsureTransactionIsStarted();
            _session.Flush();
        }

        public virtual void Commit()
        {
            try
            {
            	Flush();
                _transaction.Commit();
            }
            catch
            {
                _transaction.Rollback();
                throw;
            }
            finally
            {
                _transaction.Dispose();
                _transaction = null;
				_session.Dispose();
            	_session = null;
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
			if (disposing && CommitMode == CommitMode.Implicit)
			{
				Commit();
			}
        }

		private void EnsureTransactionIsStarted()
		{
			AssertSessionIsOpen();
			if (!IsTransactionActive)
			{
				_transaction = _session.BeginTransaction(IsolationLevel.ReadCommitted);
			}
		}

		private void AssertSessionIsOpen()
		{
			if (_session == null || !_session.IsOpen)
			{
				throw new InvalidOperationException(
					@"The unit of work's unerlying session has not been opened, or the " +
					@"session was closed. Did you forget to call UOW Start?");
			}
		}
    }
}
