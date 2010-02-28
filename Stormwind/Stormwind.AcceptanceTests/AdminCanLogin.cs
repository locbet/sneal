using System;
using System.IO;
using System.Net;
using System.Reflection;
using NUnit.Framework;
using Sneal.Core.IO;
using Sneal.TestUtils.Web;
using WatiN.Core;

namespace Stormwind.AcceptanceTests
{
	[TestFixture]
	public class AdminCanLogin
	{
		private readonly PathBuilder _pathBuilder = new PathBuilder();
		private WebDevWebServer _webServer;
		private Browser _browser;

		[SetUp]
		public void SetUp()
		{
			_webServer = WebDevWebServer.Vs2010WithDotNet40(GetWebRootDirectory());
			_webServer.Start();
			_browser = new IE(_webServer.WebRootHttpPath);
		}

		[Test]
		public void Execute()
		{
			string adminEmail = GetAdminEmail();
			string adminPassword = "password";

			_browser.GoTo(LoginUri());
			_browser.TextField(o => o.Id == "emailAddress").TypeText(adminEmail);
			_browser.TextField(o => o.Id == "password").TypeText(adminPassword);
			_browser.Forms[0].Submit();
			_browser.Text.Should().Contain("Welcome " + adminEmail);
		}

		[TearDown]
		public void Dispose()
		{
			_webServer.Dispose();
		}

		private Uri LoginUri()
		{
			return new Uri(_pathBuilder.Combine(_webServer.WebRootHttpPath, "/Account/LogOn"));
		}

		private static string GetAdminEmail()
		{
			return @"admin@" + Dns.GetHostEntry("localhost").HostName.ToLowerInvariant();
		}

		/// <summary>
		/// The Stormwind webroot directory relative to the test bin\debug folder.
		/// </summary>
		private string GetWebRootDirectory()
		{
			return _pathBuilder.Combine(
				GetCurrentExecutingDirectory(),
				@"\..\..\..\Stormwind");
		}

		/// <summary>
		/// Gets the current assembly location (pre-shadow copy).
		/// </summary>
		/// <returns>The full local directory path</returns>
		private static string GetCurrentExecutingDirectory()
		{
			string filePath = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
			return Path.GetDirectoryName(filePath);
		}
	}
}


