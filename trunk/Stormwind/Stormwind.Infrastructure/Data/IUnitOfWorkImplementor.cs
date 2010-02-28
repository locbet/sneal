using NHibernate;

namespace Stormwind.Infrastructure.Data
{
    /// <summary>
    /// An NHibernate specific unit of work implementor.
    /// </summary>
    public interface IUnitOfWorkImplementor : IUnitOfWork
    {
        /// <summary>
        /// The open NHibernate session associated with the current UOW.
        /// </summary>
        ISession Session { get; }
    }
}
