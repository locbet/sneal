using NHibernate;

namespace Stormwind.Infrastructure.Data
{
	/// <summary>
	/// Provides access to the underlying NHibernate session factory for session
	/// creation.
	/// </summary>
	public interface ISessionProvider
	{
		/// <summary>
		/// Creates a new open NHibernate session.
		/// </summary>
		ISession CreateSession();

		/// <summary>
		/// Drops (if needed), then creates the underlying NHibernate database.
		/// </summary>
		void BuildSchema();
	}
}