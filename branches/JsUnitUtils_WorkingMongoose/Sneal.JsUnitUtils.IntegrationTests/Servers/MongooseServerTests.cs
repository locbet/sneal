using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using NUnit.Framework;
using Sneal.JsUnitUtils.Servers;
using Sneal.JsUnitUtils.Utils;

namespace Sneal.JsUnitUtils.IntegrationTests.Servers
{
    [TestFixture]
    public class MongooseServerTests
    {
        private string webRootDirectory;

        [SetUp]
        public void SetUp()
        {
            webRootDirectory = AppDomain.CurrentDomain.BaseDirectory + @"\..\..\";
        }

        [Test]
        public void Can_start_http_server()
        {
            MongooseServer mg = new MongooseServer(new DiskProviderImpl(), webRootDirectory, 59998);
            using (mg.Start())
            {
                Thread.Sleep(1000);
            }
        }

        [Test]
        public void Can_start_multiple_http_servers()
        {
            MongooseServer mg = new MongooseServer(new DiskProviderImpl(), webRootDirectory, 59998);
            using (mg.Start())
            {
                Thread.Sleep(1000);
                MongooseServer mg2 = new MongooseServer(new DiskProviderImpl(), webRootDirectory, 59999);
                using (mg2.Start())
                {
                    Thread.Sleep(1000);
                }
            }
        }

        [Test]
        public void Can_serve_html_page()
        {
            MongooseServer mg = new MongooseServer(new DiskProviderImpl(), webRootDirectory, 59999);
            using (mg.Start())
            {
                string localHtmlPath = Path.Combine(webRootDirectory, "JsUnitTests\\JsUnitTestFixture1.htm");
                string httpPath = mg.MakeHttpUrl(localHtmlPath);
                Console.WriteLine("GET Local: " + localHtmlPath);
                Console.WriteLine("GET Http: " + httpPath);

                WebRequest request = WebRequest.Create(mg.MakeHttpUrl(localHtmlPath));
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
}