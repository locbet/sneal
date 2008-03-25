//=====================================================================
//
// THIS CODE AND INFORMATION IS PROVIDED TO YOU FOR YOUR REFERENTIAL
// PURPOSES ONLY, AND IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE,
// AND MAY NOT BE REDISTRIBUTED IN ANY MANNER.
//
// Copyright (C) 2003  Microsoft Corporation.  All rights reserved.
//
//=====================================================================
using System;
using System.Collections;

namespace SqlAdmin 
{
	/// <summary>
	/// A collection of SqlView objects that represent the views in a SQL database.
	/// </summary>
	public class SqlViewCollection : ICollection 
	{
		private ArrayList views;
		private SqlDatabase database;


		internal SqlViewCollection(SqlDatabase database) 
		{
			this.database = database;
		}

		/// <summary>
		/// Gets the number of views in the SqlViewCollection.
		/// </summary>
		public int Count 
		{
			get 
			{
				if (views != null)
					return views.Count;
				else
					return 0;
			}
		}

		/// <summary>
		/// Gets a value that indicates whether the columns in the SqlViewCollection can be modified.
		/// </summary>
		public bool IsReadOnly 
		{
			get 
			{
				return false;
			}
		}

		/// <summary>
		/// Gets a value indicating whether access to the SqlViewCollection is synchronized (thread-safe).
		/// </summary>
		public bool IsSynchronized 
		{
			get 
			{
				return false;
			}
		}

		/// <summary>
		/// Gets the object that can be used to synchronize access to the SqlViewCollection.
		/// </summary>
		public object SyncRoot 
		{
			get 
			{
				return this;
			}
		}

		/// <summary>
		/// Gets a SqlView object from the SqlViewCollection collection at the specified index.
		/// </summary>
		public SqlView this[int index] 
		{
			get 
			{
				if (views != null)
					return (SqlView)(views[index]);
				else
					return null;
			}
		}

		/// <summary>
		/// Gets a SqlView object from the SqlViewCollection collection that has the specified name (case-insensitive).
		/// </summary>
		public SqlView this[string name] 
		{
			get 
			{
				if (views != null) 
				{
					for (int i = 0; i < views.Count; i++) 
					{
						if (name.ToLower() == ((SqlView)views[i]).Name.ToLower())
							return (SqlView)(views[i]);
					}
				}

				// If there is no view list, or the name does not exist, return null
				return null;
			}
		}

		/// <summary>
		/// Copies the items from the SqlViewCollection to the specified System.Array object, starting at the specified index in the System.Array object.
		/// </summary>
		/// <param name="array">
		/// </param>
		/// <param name="index">
		/// </param>
		public void CopyTo(Array array, int index) 
		{
			for (IEnumerator e = this.GetEnumerator(); e.MoveNext();)
				array.SetValue(e.Current, index++);
		}

		/// <summary>
		/// Returns an System.Collections.IEnumerator interface that contains all SqlView objects in the SqlViewCollection.
		/// </summary>
		/// <returns>
		/// A System.Collections.IEnumerator interface that contains all SqlView objects in the SqlViewCollection.
		/// </returns>
		public IEnumerator GetEnumerator() 
		{
			if (views == null) 
			{
				views = new ArrayList();
			}

			return views.GetEnumerator(0, Count);
		}

		/// <summary>
		/// Updates the SqlViewCollection with any changes made since the last call to Refresh.
		/// Refresh is automatically called once when the SqlDatabase.Tables collection is read.
		/// </summary>
		public void Refresh() 
		{
			// Force internal refresh of views
			database.dmoDatabase.GetViews().Refresh(false);

			// Clear out old list
			views = new ArrayList();

			for (int i = 0; i < database.dmoDatabase.GetViews().GetCount(); i++) 
			{
				NativeMethods.IView dmoView = database.dmoDatabase.GetViews().Item(i + 1, "");

				SqlView view;

				if (dmoView.GetSystemObject())
					view = new SqlView(dmoView.GetName(), dmoView.GetOwner(), SqlObjectType.System, DateTime.Parse(dmoView.GetCreateDate()));
				else
					view = new SqlView(dmoView.GetName(), dmoView.GetOwner(), SqlObjectType.User, DateTime.Parse(dmoView.GetCreateDate()));

				views.Add(view);

				view.dmoView = dmoView;
				view.database = this.database;
			}
		}
	}
}
