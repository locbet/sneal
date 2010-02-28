using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Stormwind.Core.Repositories
{
    /// <summary>
    /// Base repository type.  Makes declaring concrete repository interfaces easier.
    /// </summary>
    /// <typeparam name="TEntity">The entity model type</typeparam>
    public interface IRepository<TEntity>
    {
        /// <summary>
        /// Gets the entity by it's primary key identifier.
        /// </summary>
        /// <param name="id">The id of the entity</param>
        /// <returns>The hydrated instance</returns>
        TEntity Get(Guid id);

        /// <summary>
        /// Gets the first entity that matches the specified filter, or the
        /// default if no entity matches the specified filter.
        /// </summary>
        /// <param name="filter">The LINQ query filter</param>
        /// <returns>The first entity to match the filter</returns>
        TEntity GetByFilter(Expression<Func<TEntity, bool>> filter);

        /// <summary>
        /// Gets all the entities that matches the specified filter, or the
        /// default if no entity matches the specified filter.
        /// </summary>
        /// <param name="filter">The LINQ query filter</param>
        /// <returns>The first entity to match the filter</returns>
        IEnumerable<TEntity> GetAllByFilter(Expression<Func<TEntity, bool>> filter);

        /// <summary>
        /// Registers the specified instance for save or update.
        /// </summary>
        /// <param name="entity">The entity instance to persist</param>
        /// <returns>The same instance</returns>
        TEntity Put(TEntity entity);

        /// <summary>
        /// Registers the specified entity for deletion.
        /// </summary>
        /// <param name="entity">The entity to remove</param>
        void Remove(TEntity entity);

        /// <summary>
        /// Registers the specified entities for deletion.
        /// </summary>
        /// <param name="entitiesToDelete">The entities to delete</param>
        void Remove(IEnumerable<TEntity> entitiesToDelete);

        /// <summary>
        /// Registers the entities that match the specified filter for deletion.
        /// </summary>
        /// <param name="filter">The delete filter</param>
        void Remove(Expression<Func<TEntity, bool>> filter);
    }
}
