using System;

namespace SqlAdmin
{
	/// <summary>
	/// Represents a SQL view.
	/// </summary>
	public class SqlView
	{
		internal NativeMethods.IView dmoView = null;
		internal SqlDatabase database = null;

		private string _name;
		private string _owner;
		private SqlObjectType _viewType;
		private DateTime _createDate;

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlView"/> class.
		/// </summary>
		/// <param _name="_name">The _name.</param>
		/// <param _name="_owner">The _owner.</param>
		/// <param _name="viewType">Type of the view.</param>
		/// <param _name="createDate">The create date.</param>
		internal SqlView(string name, string owner, SqlObjectType viewType, DateTime createDate)
		{
			_name = name;
			_owner = owner;
			_viewType = viewType;
			_createDate = createDate;
		}

		/// <summary>
		/// The SqlDatabase to which this table belongs.
		/// </summary>
		public SqlDatabase Database 
		{
			get 
			{
				return database;
			}
		}

		/// <summary>
		/// The _name of the table.
		/// </summary>
		public string Name 
		{
			get { return _name; }
			set
			{
				// Re_name both the DMO view and the internal _name
				dmoView.SetName(value);
				_name = value;
			}
		}

		/// <summary>
		/// The date and time when this table was created.
		/// </summary>
		public DateTime CreateDate 
		{
			get 
			{
				return _createDate;
			}
		}

		/// <summary>
		/// The _owner of the table.
		/// </summary>
		public string Owner 
		{
			get { return _owner; }
		}

		/// <summary>
		/// Scripts the specified script type.
		/// </summary>
		/// <param _name="scriptType">Script options.</param>
		/// <returns>T-SQL script.</returns>
		public string Script(SqlScriptType scriptType)
		{
			int dmoScriptType = 0;
			dmoScriptType |= NativeMethods.SQLDMO_SCRIPT_TYPE.SQLDMOScript_Default
				| NativeMethods.SQLDMO_SCRIPT_TYPE.SQLDMOScript_OwnerQualify;

			if ((scriptType & SqlScriptType.Drop) == SqlScriptType.Drop)
				dmoScriptType |= NativeMethods.SQLDMO_SCRIPT_TYPE.SQLDMOScript_Drops;

			if ((scriptType & SqlScriptType.Comments) == SqlScriptType.Comments)
				dmoScriptType |= NativeMethods.SQLDMO_SCRIPT_TYPE.SQLDMOScript_IncludeHeaders;

			if ((scriptType & SqlScriptType.Permissions) == SqlScriptType.Permissions)
				dmoScriptType |= NativeMethods.SQLDMO_SCRIPT_TYPE.SQLDMOScript_ObjectPermissions;

			return dmoView.Script(dmoScriptType, null, 0);
		}
	}
}
