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

namespace SqlAdmin {

	/// <summary>
	/// Privledge types
	/// </summary>
	[Flags]
	public enum SqlPrivilegeType
	{
		/// <summary>Unknown</summary>
		Unknown = 0,
		/// <summary>Select</summary>
		Select = 1,
		/// <summary>Insert</summary>
		Insert = 2,
		/// <summary>Update</summary>
		Update = 4,
		/// <summary>Delete</summary>
		Delete = 8,
		/// <summary>Execute</summary>
		Execute = 16,
		/// <summary>References</summary>
		References = 32,
		/// <summary>AllObjectPrivs</summary>
		AllObjectPrivs = 63,
		/// <summary>CreateTable</summary>
		CreateTable = 128,
		/// <summary>CreateDatabase</summary>
		CreateDatabase = 256,
		/// <summary>CreateView</summary>
		CreateView = 512,
		/// <summary>CreateProcedure</summary>
		CreateProcedure = 1024,
		/// <summary>DumpDatabase</summary>
		DumpDatabase = 2048,
		/// <summary>CreateDefault</summary>
		CreateDefault = 4096,
		/// <summary>DumpTransaction</summary>
		DumpTransaction = 8192,
		/// <summary>CreateRule</summary>
		CreateRule = 16384,
		/// <summary>DumpTable</summary>
		DumpTable = 32768,
		/// <summary>CreateFunction</summary>
		CreateFunction = 0x00010000,
		/// <summary>AllDatabasePrivs</summary>
		AllDatabasePrivs = 0x0001ff80,

	}
}
