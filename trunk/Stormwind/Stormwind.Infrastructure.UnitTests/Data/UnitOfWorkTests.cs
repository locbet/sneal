using System;
using System.Data;
using NHibernate;
using NUnit.Framework;
using Rhino.Mocks;
using Stormwind.Infrastructure.Data;

namespace Stormwind.Infrastructure.UnitTests.Data
{
	[TestFixture]
	public class UnitOfWorkTests
	{
		private ISessionProvider _sessionProvider;
		private ITransaction _transaction;
		private ISession _session;

		[Test]
		public void IsTransactionActive_defaults_to_false()
		{
			UnitOfWork uow = CreateUnitOfWork();
			uow.IsTransactionActive.Should().Be.False();
		}

		[Test]
		public void IsTransactionActive_is_false_after_Start()
		{
			UnitOfWork uow = CreateUnitOfWork();
			uow.Start();
			uow.IsTransactionActive.Should().Be.False();
		}

		[Test]
		public void IsTransactionActive_is_true_after_Flush()
		{
			UnitOfWork uow = CreateUnitOfWork();
			uow.Start();
			uow.Flush();
			uow.IsTransactionActive.Should().Be.True();
		}

		[Test]
		public void Start_creates_session()
		{
			UnitOfWork uow = CreateUnitOfWork();
			Assert.IsNull(uow.Session);
			uow.Start();
			Assert.IsNotNull(uow.Session);
		}

		[Test]
		public void Start_does_not_create_new_session_if_uncommitted()
		{
			UnitOfWork uow = CreateUnitOfWork();
			uow.Start();
			uow.Start();
			_sessionProvider.AssertWasCalled(o => o.CreateSession(), m => m.Repeat.Once());
		}

		[Test]
		public void Start_creates_a_new_session_after_commit_is_called()
		{
			UnitOfWork uow = CreateUnitOfWork();
			uow.Start();
			uow.Commit();
			uow.Start();
			_sessionProvider.AssertWasCalled(o => o.CreateSession(), m => m.Repeat.Twice());
		}

		[Test]
		public void Start_does_not_create_a_transaction()
		{
			UnitOfWork uow = CreateUnitOfWork();
			uow.Start();
			_session.AssertWasNotCalled(o => o.BeginTransaction(IsolationLevel.ReadCommitted));
		}

		[Test]
		public void Flush_throws_InvalidOperationException_if_not_started()
		{
			UnitOfWork uow = CreateUnitOfWork();
			Assert.Throws<InvalidOperationException>(uow.Flush);
		}

		[Test]
		public void Flush_starts_a_ReadCommitted_transaction()
		{
			UnitOfWork uow = CreateUnitOfWork();
			uow.Start();
			uow.Flush();
			_session.AssertWasCalled(o => o.BeginTransaction(IsolationLevel.ReadCommitted));
		}

		[Test]
		public void Flush_flushes_the_underlying_session()
		{
			UnitOfWork uow = CreateUnitOfWork();
			uow.Start();
			uow.Flush();
			_session.AssertWasCalled(o => o.Flush());
		}

		[Test]
		public void CommitMode_defaults_to_explicit()
		{
			UnitOfWork uow = CreateUnitOfWork();
			uow.CommitMode.Should().Be.EqualTo(CommitMode.Explicit);
		}

		[Test]
		public void Commit_calls_session_flush_and_transaction_commit()
		{
			UnitOfWork uow = CreateUnitOfWork();
			uow.Start();
			uow.Commit();

			_session.AssertWasCalled(o => o.Flush());
			_transaction.AssertWasCalled(o => o.Commit());
		}

		private UnitOfWork CreateUnitOfWork()
		{
			_sessionProvider = MockRepository.GenerateStub<ISessionProvider>();
			_session = MockRepository.GenerateStub<ISession>();
			_transaction = MockRepository.GenerateStub<ITransaction>();

			_transaction.Stub(o => o.IsActive).Return(true);
			_session.Stub(o => o.IsOpen).Return(true);
			_session.Stub(o => o.BeginTransaction(0)).IgnoreArguments().Return(_transaction);
			_sessionProvider.Stub(o => o.CreateSession()).Return(_session);

			return new UnitOfWork(_sessionProvider);
		}
	}
}
