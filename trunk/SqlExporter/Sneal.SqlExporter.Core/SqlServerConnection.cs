using System;
using SqlAdmin;

namespace Sneal.SqlExporter
{
	/// <summary>
	/// IDispoable wrapper around SqlAdmin.SqlServer
	/// </summary>
	public class SqlServerConnection : SqlServer, IDisposable
	{
		// has this been garbage collected?
		private bool _disposed = false;

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlServerConnection"/> class.
		/// Use this constructor when needing to use integrated authentication under
		/// a different user, i.e. a different NT user then this app is running
		/// under.
		/// </summary>
		/// <param name="server">The server.</param>
		/// <param name="userName">Name of the user.</param>
		/// <param name="password">The password.</param>
		/// <param name="useImpersonation">if set to <c>true</c> [use impersonation].</param>
		public SqlServerConnection(string server, string userName, string password, bool useImpersonation)
			: base(server, userName, password, !useImpersonation) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlServerConnection"/> class.
		/// Use this when using SQL Authentication.
		/// </summary>
		/// <param name="server">The server.</param>
		/// <param name="userName">Name of the user.</param>
		/// <param name="password">The password.</param>
		public SqlServerConnection(string server, string userName, string password)
			: base(server, userName, password, true) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlServerConnection"/> class.
		/// This constructor assumes NT Integrated Authentication.
		/// </summary>
		/// <param name="server">The server.</param>
		public SqlServerConnection(string server)
			: base(server)
		{
			base.LoginSecure = true;
		}

		/// <summary>
		/// Gets a database reference.
		/// </summary>
		/// <param name="name">The name of the database to get.</param>
		/// <returns></returns>
		public SqlDatabase GetDatabase(string name)
		{
			return this.Databases[name];
		}

		#region IDisposable Members

		/// Ensure you call this when you are finished impersonating the
		/// user.  This will logoff the runas user and revert subsequent
		/// code to the original user context.  Same as Logout().
		public void Dispose()
		{
			// free resources
			Dispose(true);

			// object resource were already freed, remove
			// this object from the GC finalizer list
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Does the actual resource freeing
		/// </summary>
		/// <param name="disposing">False if method called from the finalizer</param>
		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				//if (disposing)
				//	base.Disconnect();
			}

			_disposed = true;
		}

		#endregion
	}
}
