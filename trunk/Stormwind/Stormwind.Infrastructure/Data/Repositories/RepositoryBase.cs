using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate;
using NHibernate.Linq;
using Sneal.Core.Collections;
using Stormwind.Core.Repositories;

namespace Stormwind.Infrastructure.Data.Repositories
{
	/// <summary>
	/// A generic base repository implmentation for NHibernate.
	/// </summary>
	/// <typeparam name="TEntity">The domain entity type</typeparam>
	public abstract class RepositoryBase<TEntity> : IRepository<TEntity>
	{
		private readonly IUnitOfWorkImplementor _unitOfWork;

		/// <summary>
		/// Creates a new repository instance.
		/// </summary>
		/// <param name="unitOfWork">The current open NHibernate UOW</param>
		protected RepositoryBase(IUnitOfWorkImplementor unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		/// <summary>
		/// The NHibernate session associated with the current unit of work.
		/// </summary>
		protected ISession Session
		{
			get { return _unitOfWork.Session; }
		}

		/// <summary>
		/// Gets the entity by it's primary key identifier.
		/// </summary>
		/// <param name="id">The id of the entity</param>
		/// <returns>The hydrated instance</returns>
		public virtual TEntity Get(Guid id)
		{
			return Session.Load<TEntity>(id);
		}

		/// <summary>
		/// Gets the first entity that matches the specified filter, or the
		/// default if no entity matches the specified filter.
		/// </summary>
		/// <param name="filter">The LINQ query filter</param>
		/// <returns>The first entity to match the filter</returns>
		public virtual TEntity GetByFilter(Expression<Func<TEntity, bool>> filter)
		{
			return GetAllByFilter(filter).FirstOrDefault();
		}

		/// <summary>
		/// Gets all the entities that matches the specified filter, or the
		/// default if no entity matches the specified filter.
		/// </summary>
		/// <param name="filter">The LINQ query filter</param>
		/// <returns>The first entity to match the filter</returns>
		public virtual IEnumerable<TEntity> GetAllByFilter(Expression<Func<TEntity, bool>> filter)
		{
			return Session.Linq<TEntity>().Where(filter);
		}

		/// <summary>
		/// Registers the specified instance for save or update.
		/// </summary>
		/// <param name="entity">The entity instance to persist</param>
		/// <returns>The same instance</returns>
		public virtual TEntity Put(TEntity entity)
		{
			Session.SaveOrUpdate(entity);
			return entity;
		}

		/// <summary>
		/// Registers the specified entity for deletion.
		/// </summary>
		/// <param name="entity">The entity to remove</param>
		public virtual void Remove(TEntity entity)
		{
			Session.Delete(entity);
		}

		/// <summary>
		/// Registers the specified entities for deletion.
		/// </summary>
		/// <param name="entitiesToDelete">The entities to delete</param>
		public virtual void Remove(IEnumerable<TEntity> entitiesToDelete)
		{
			entitiesToDelete.ForEach(Remove);
		}

		/// <summary>
		/// Registers the entities that match the specified filter for deletion.
		/// </summary>
		/// <param name="filter">The delete filter</param>
		public virtual void Remove(Expression<Func<TEntity, bool>> filter)
		{
			Remove(GetAllByFilter(filter));
		}
	}
}