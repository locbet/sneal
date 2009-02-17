#region license
// Copyright 2008 Shawn Neal (sneal@sneal.net)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.IO;
using System.Net;
using System.Text;
using NUnit.Framework;
using Sneal.JsUnitUtils.Servers;
using Sneal.JsUnitUtils.Utils;

namespace Sneal.JsUnitUtils.IntegrationTests.Servers
{
    [TestFixture]
    public class BuiltinWebServerTests
    {
        private string webRootDirectory;

        [SetUp]
        public void SetUp()
        {
            webRootDirectory = AppDomain.CurrentDomain.BaseDirectory + @"\..\..\";
        }

        /// <summary>
        /// This tests that more than one page can be served.  Initially this
        /// was failing because a new begin get context was not created 
        /// immediately after an initial request.
        /// </summary>
        [Test]
        public void Can_serve_multiple_html_pages()
        {
            BuiltinWebServer mg = new BuiltinWebServer(new DiskProviderImpl(), webRootDirectory, 59999);
            using (mg.Start())
            {
                string localHtmlPath1 = Path.Combine(webRootDirectory, "JsUnitTests\\JsUnitTestFixture1.htm");
                string httpPath1 = mg.MakeHttpUrl(localHtmlPath1);
                RequestAndAssertPage(httpPath1);

                string localHtmlPath2 = Path.Combine(webRootDirectory, "JsUnitTests\\JsUnitTestFixture2.htm");
                string httpPath2 = mg.MakeHttpUrl(localHtmlPath2);
                RequestAndAssertPage(httpPath2);
            }
        }

        /// <summary>
        /// Gets the page via the Http server and asserts something is returned.
        /// </summary>
        private static void RequestAndAssertPage(string htmlPageUrl)
        {
            WebRequest request = WebRequest.Create(htmlPageUrl);
            using (WebResponse myWebResponse = request.GetResponse())
            {
                Stream ReceiveStream = myWebResponse.GetResponseStream();
                using (StreamReader readStream = new StreamReader(ReceiveStream, Encoding.UTF8))
                {
                    string strResponse = readStream.ReadToEnd();
                    Console.WriteLine(strResponse);
                    Assert.IsNotNull(strResponse, "Null response returned");
                    Assert.IsTrue(strResponse.Contains("<html>"), "No HTML element found");
                }
            }            
        }
    }
}
