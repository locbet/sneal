using System;
using System.Threading;
using NUnit.Framework;
using Sneal.JsUnitUtils.Browsers;

namespace JsUnitUtils.Tests.Browsers
{
    [TestFixture]
    public class InternetExplorerBrowserTests
    {
        [Test]
        public void Should_open_browser_then_close()
        {
            InternetExplorerBrowser browser = new InternetExplorerBrowser();
            browser.OpenUrl(new Uri("http://google.com"));
            Thread.Sleep(10000);
            browser.Close();
        }
    }
}
