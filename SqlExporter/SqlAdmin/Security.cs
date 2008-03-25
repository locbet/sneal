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
using System.Security;
using System.Security.Principal;
using System.Web.Security;
using System.Web;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace SqlAdmin
{
	/// <summary>
	/// General Security functions.
	/// </summary>
	public class Security
	{
		private WindowsIdentity identity;
		private WindowsImpersonationContext impersonatedUser;

		/// <summary>
		/// Initializes a new instance of the <see cref="Security"/> class.
		/// </summary>
		public Security() { }

		/// <summary>
		/// The current identity of the user
		/// </summary>
		public WindowsIdentity  Identity
		{
			get { return identity; }
			set { identity = value; }
		}

		#region Methods

		/// <summary>
		/// Starts the impersonation process. If an exception occurs it is rolled up to the caller.
		/// </summary>
		/// <param name="username">Username</param>
		/// <param name="domain">domain</param>
		/// <param name="password">password</param>
		/// <returns>Indicates whether the impersonation process was successful.</returns>
		public bool Impersonate(string username, string domain, string password)
		{
			try
			{
				return this.DoImpersonate(username, domain, password);
			}
			catch (Exception)
			{
				throw;
			}
		}		

		/// <summary>
		/// Ends impersonation 
		/// </summary>
		public void EndImpersonate()
		{
			this.StopImpersonate();
		}
		#endregion

		#region interop/unsafe methods

		IntPtr tokenHandle = new IntPtr(0);
		IntPtr dupeTokenHandle = new IntPtr(0);

		/// <summary>
		/// Logons the user.
		/// </summary>
		/// <param name="lpszUsername">The LPSZ username.</param>
		/// <param name="lpszDomain">The LPSZ domain.</param>
		/// <param name="lpszPassword">The LPSZ password.</param>
		/// <param name="dwLogonType">Type of the dw logon.</param>
		/// <param name="dwLogonProvider">The dw logon provider.</param>
		/// <param name="phToken">The ph token.</param>
		/// <returns></returns>
		[DllImport("advapi32.dll", SetLastError=true)]
		public static extern bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword, 
			int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

		/// <summary>
		/// Formats the message.
		/// </summary>
		/// <param name="dwFlags">The dw flags.</param>
		/// <param name="lpSource">The lp source.</param>
		/// <param name="dwMessageId">The dw message id.</param>
		/// <param name="dwLanguageId">The dw language id.</param>
		/// <param name="lpBuffer">The lp buffer.</param>
		/// <param name="nSize">Size of the n.</param>
		/// <param name="Arguments">The arguments.</param>
		/// <returns></returns>
		[DllImport("kernel32.dll", CharSet=System.Runtime.InteropServices.CharSet.Auto)]
		private unsafe static extern int FormatMessage(int dwFlags, ref IntPtr lpSource, 
			int dwMessageId, int dwLanguageId, ref String lpBuffer, int nSize, IntPtr *Arguments);

		/// <summary>
		/// Closes the handle.
		/// </summary>
		/// <param name="handle">The handle.</param>
		/// <returns></returns>
		[DllImport("kernel32.dll", CharSet=CharSet.Auto)]
		public extern static bool CloseHandle(IntPtr handle);

		/// <summary>
		/// Duplicates the token.
		/// </summary>
		/// <param name="ExistingTokenHandle">The existing token handle.</param>
		/// <param name="SECURITY_IMPERSONATION_LEVEL">The SECURIT y_ IMPERSONATIO n_ LEVEL.</param>
		/// <param name="DuplicateTokenHandle">The duplicate token handle.</param>
		/// <returns></returns>
		[DllImport("advapi32.dll", CharSet=CharSet.Auto, SetLastError=true)]
		public extern static bool DuplicateToken(IntPtr ExistingTokenHandle, 
			int SECURITY_IMPERSONATION_LEVEL, ref IntPtr DuplicateTokenHandle);

		/// <summary>
		/// GetErrorMessage formats and returns an error message
		/// corresponding to the input errorCode.
		/// </summary>
		/// <param name="errorCode">The error code.</param>
		/// <returns></returns>
		public unsafe static string GetErrorMessage(int errorCode) {
			int FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x00000100;
			int FORMAT_MESSAGE_IGNORE_INSERTS = 0x00000200;
			int FORMAT_MESSAGE_FROM_SYSTEM  = 0x00001000;

			//int errorCode = 0x5; //ERROR_ACCESS_DENIED
			//throw new System.ComponentModel.Win32Exception(errorCode);

			int messageSize = 255;
			String lpMsgBuf = "";
			int dwFlags = FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS;

			IntPtr ptrlpSource = IntPtr.Zero;
			IntPtr prtArguments = IntPtr.Zero;
        
			int retVal = FormatMessage(dwFlags, ref ptrlpSource, errorCode, 0, ref lpMsgBuf, messageSize, &prtArguments);
			if (0 == retVal) {
				throw new Exception("Failed to format message for error code " + errorCode + ". ");
			}

			return lpMsgBuf;
		} 

		/// <summary>
		/// Starts the impersonation process
		/// </summary>
		/// <param name="username">The username</param>
		/// <param name="domain">The domain name</param>
		/// <param name="password">The password</param>
		/// <returns></returns>
		public  bool DoImpersonate(string username, string domain,string password)
		{
			if ( System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToLower() == domain + @"\" + username )
				return true;

			const int LOGON32_PROVIDER_DEFAULT = 0;
			//This parameter causes LogonUser to create a primary token.
			const int LOGON32_LOGON_INTERACTIVE = 2;
			const int SecurityImpersonation = 2;

			tokenHandle = IntPtr.Zero;
			dupeTokenHandle = IntPtr.Zero;

			// Call LogonUser to obtain a handle to an access token.
			bool returnValue = LogonUser(username, domain, password, 
				LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT,
				ref tokenHandle);

			if (false == returnValue) {
				int ret = Marshal.GetLastWin32Error();
				int errorCode = 0x5; //ERROR_ACCESS_DENIED
				throw new System.ComponentModel.Win32Exception(errorCode);
			}

			bool retVal = DuplicateToken(tokenHandle, SecurityImpersonation, ref dupeTokenHandle);

			if (false == retVal) {
				CloseHandle(tokenHandle);
				return retVal;
			}

			// The token that is passed to the following constructor must 
			// be a primary token in order to use it for impersonation.
			WindowsIdentity newId = new WindowsIdentity(dupeTokenHandle);
			impersonatedUser = newId.Impersonate();

			return retVal;
		}


		/// <summary>
		/// Stops the impersonation process
		/// </summary>
		public  void StopImpersonate() 
		{
			//impersonatedUser.Undo();
			// Free the tokens.
			if (tokenHandle != IntPtr.Zero)
				CloseHandle(tokenHandle);
			if (dupeTokenHandle != IntPtr.Zero) 
				CloseHandle(dupeTokenHandle);
		}


//		public const int LOGON32_LOGON_INTERACTIVE = 2;
//		public const int LOGON32_PROVIDER_DEFAULT = 0;
//
//		WindowsImpersonationContext impersonationContext; 
//
//		[DllImport("advapi32.dll", CharSet=CharSet.Auto)]
//		private static extern int LogonUser(String lpszUserName, 
//			String lpszDomain,
//			String lpszPassword,
//			int dwLogonType, 
//			int dwLogonProvider,
//			ref IntPtr phToken);
//		[DllImport("advapi32.dll", CharSet=System.Runtime.InteropServices.CharSet.Auto,
//			 SetLastError=true)]
//		private extern static int DuplicateToken(IntPtr hToken, 
//			int impersonationLevel,  
//			ref IntPtr hNewToken);
//
//
//		private bool StartImpersonation(String userName, String domain, String password) {
//			WindowsIdentity tempWindowsIdentity;
//			IntPtr token = IntPtr.Zero;
//			IntPtr tokenDuplicate = IntPtr.Zero;
//
//			if(LogonUser(userName, domain, password, LOGON32_LOGON_INTERACTIVE, 
//				LOGON32_PROVIDER_DEFAULT, ref token) != 0) {
//				if(DuplicateToken(token, 2, ref tokenDuplicate) != 0) {
//					tempWindowsIdentity = new WindowsIdentity(tokenDuplicate);
//					impersonationContext = tempWindowsIdentity.Impersonate();
//					if (impersonationContext != null)
//						return true;
//					else
//						return false; 
//				}
//				else
//					return false;
//			} 
//			else
//				return false;
//		}
//		private void EndImpersonation() {
//			// Throws exception
//			try {
//				impersonationContext.Undo();
//			} catch {}
//		} 

		#endregion

	}
}
