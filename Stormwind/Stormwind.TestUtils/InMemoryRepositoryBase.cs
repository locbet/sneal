using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Sneal.Core.Collections;
using Stormwind.Core.Models;
using Stormwind.Core.Repositories;

namespace Stormwind.TestUtils
{
	/// <summary>
	/// Stubbed out in memory repository that keeps state between calls.
	/// </summary>
    public class InMemoryRepositoryBase<TEntity> : IRepository<TEntity>
    {
        private readonly Dictionary<Guid, TEntity> _users = new Dictionary<Guid, TEntity>();

        public TEntity Get(Guid id)
        {
            return _users[id];
        }

        public TEntity GetByFilter(Expression<Func<TEntity, bool>> filter)
        {
            return GetAllByFilter(filter).FirstOrDefault();
        }

        public IEnumerable<TEntity> GetAllByFilter(Expression<Func<TEntity, bool>> filter)
        {
            return _users.Values.Where(filter.Compile());
        }

        public TEntity Put(TEntity entity)
        {
            return _users[GetId(entity)] = entity;
        }

        public void Remove(TEntity entity)
        {
            _users.Remove(GetId(entity));
        }

        public void Remove(IEnumerable<TEntity> entitiesToDelete)
        {
            entitiesToDelete.ForEach(Remove);
        }

        public void Remove(Expression<Func<TEntity, bool>> filter)
        {
            Remove(GetAllByFilter(filter));
        }

        private static Guid GetId(object entity)
        {
        	return ((IEntity) entity).Id;
        }
    }
}
