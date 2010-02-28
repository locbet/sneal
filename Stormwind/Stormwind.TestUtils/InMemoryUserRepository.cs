using Stormwind.Core.Models;
using Stormwind.Core.Repositories;

namespace Stormwind.TestUtils
{
    public class InMemoryUserRepository : InMemoryRepositoryBase<User>, IUserRepository
    {
    }
}
