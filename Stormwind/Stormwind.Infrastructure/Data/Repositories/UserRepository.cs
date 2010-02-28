using Stormwind.Core.Models;
using Stormwind.Core.Repositories;
using Stormwind.Infrastructure.Data;
using Stormwind.Infrastructure.Data.Repositories;

namespace Stormwind.Infrastructure.Repositories
{
    /// <summary>
    /// NHibernate implementation of IUserRepository.
    /// </summary>
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        /// <summary>
        /// Creates a new repository instance.
        /// </summary>
        /// <param name="unitOfWork">
        /// A delegate for getting the ISession associated with the current UOW.
        /// </param>
        public UserRepository(IUnitOfWorkImplementor unitOfWork)
            : base(unitOfWork) { }
    }
}