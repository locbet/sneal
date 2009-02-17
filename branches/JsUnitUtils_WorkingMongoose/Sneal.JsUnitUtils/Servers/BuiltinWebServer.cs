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
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using Sneal.JsUnitUtils.Utils;

namespace Sneal.JsUnitUtils.Servers
{
    public class BuiltinWebServer : IWebServer
    {
        public event DataPosted DataPostedEvent;

        private int port;
        private HttpListener httpListener;
        private IDiskProvider diskProvider;
        private string webRootDirectory;
        private static readonly object listenerLock = new object();
        private bool isRunning;

        public BuiltinWebServer(IDiskProvider diskProvider, string webRootDirectory, int port)
        {
            this.diskProvider = diskProvider;
            this.webRootDirectory = webRootDirectory;
            this.port = port;
        }

        public BuiltinWebServer(IDiskProvider diskProvider, string webRootDirectory)
        {
            this.diskProvider = diskProvider;
            this.webRootDirectory = webRootDirectory;
            port = new FreeTcpPortFinder().FindFreePort(60000);
        }

        public string WebBinDirectory
        {
            get { return Path.Combine(webRootDirectory, "bin"); }
        }

        public string WebRootHttpPath
        {
            get { return string.Format("http://localhost:{0}/", WebServerPort); }
        }

        public int WebServerPort
        {
            get { return port; }
        }

        public string WebRootDirectory
        {
            get { return webRootDirectory; }
        }

        public string MakeHttpUrl(string localFilePath)
        {
            string normalizedPath = diskProvider.NormalizePath(localFilePath);
            if (!Path.IsPathRooted(normalizedPath))
            {
                return diskProvider.Combine(WebRootHttpPath, normalizedPath);
            }

            string relativePath = diskProvider.MakePathRelative(WebRootDirectory, normalizedPath);
            return diskProvider.Combine(WebRootHttpPath, relativePath);            
        }

        public string MakeLocalPath(string httpFilePath)
        {
            if (string.IsNullOrEmpty(httpFilePath))
            {
                return "";
            }

            return diskProvider.Combine(webRootDirectory, httpFilePath);
        }

        public DisposableAction Start()
        {
            lock (listenerLock)
            {
                isRunning = true;
                httpListener = new HttpListener();
                httpListener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
                httpListener.Prefixes.Add(WebRootHttpPath);
                httpListener.Start();
            }
            CreateNewReceieveContext();
            return new DisposableAction(Stop);
        }

        public void Stop()
        {
            lock (listenerLock)
            {
                if (httpListener != null && isRunning)
                {
                    isRunning = false;
                    httpListener.Abort();
                    httpListener = null;
                }
            }
        }

        public string HandlerAddress
        {
            get { return Path.Combine(WebRootHttpPath, "jsunitresults"); }
        }

        private void CreateNewReceieveContext()
        {
            httpListener.BeginGetContext(HandleRequest, httpListener);
        }

        private void HandleGetRequest(HttpListenerContext context)
        {
            string filePath = context.Request.Url.LocalPath;
            string localFilePath = MakeLocalPath(filePath);
            string ext = Path.GetExtension(localFilePath).ToLowerInvariant();

            if (!File.Exists(localFilePath))
            {
                Write404(context.Response);
                return;
            }

            Debug.WriteLine("Handling " + localFilePath);

            if (ext == ".html" || ext == ".htm")
            {
                context.Response.ContentType = "text/html";
                WriteTextFileToOutput(context, localFilePath);
            }
            else if (ext == ".js")
            {
                context.Response.ContentType = "text/javascript";
                WriteTextFileToOutput(context, localFilePath);
            }
            else if (ext == ".css")
            {
                context.Response.ContentType = "text/css";
                WriteTextFileToOutput(context, localFilePath);
            }
            else
            {
                // unsupported mime type
                Write404(context.Response);
            }
        }

        private void WriteTextFileToOutput(HttpListenerContext context, string localFilePath)
        {
            using (StreamReader reader = new StreamReader(localFilePath))
            {
                byte[] bOutput = System.Text.Encoding.UTF8.GetBytes(reader.ReadToEnd());

                context.Response.ContentLength64 = bOutput.Length;

                context.Response.OutputStream.Write(bOutput, 0, bOutput.Length);
                context.Response.OutputStream.Close();
            }
            context.Response.StatusCode = 200;
        }

        private static void Write404(HttpListenerResponse response)
        {
            response.StatusCode = 404;

            const string str404 = "<html><body><h1>page not found</h1></body></html>";
            byte[] bOutput = Encoding.UTF8.GetBytes(str404);

            response.ContentType = "text/html";
            response.ContentLength64 = bOutput.Length;

            response.OutputStream.Write(bOutput, 0, bOutput.Length);
            response.OutputStream.Close();
        }

        private static void Write200(HttpListenerResponse response)
        {
            response.StatusCode = 200;

            const string str200 = "<html><body><h1>OK</h1></body></html>";
            byte[] bOutput = Encoding.UTF8.GetBytes(str200);

            response.ContentType = "text/html";
            response.ContentLength64 = bOutput.Length;

            response.OutputStream.Write(bOutput, 0, bOutput.Length);
            response.OutputStream.Close();
        }

        private void HandlePostRequest(HttpListenerContext context)
        {
            string postData;
            using (StreamReader postReader = new StreamReader(context.Request.InputStream, true))
            {
                postData = postReader.ReadToEnd();
            }

            Debug.WriteLine("POST: " + context.Request.Url);
            Write200(context.Response);

            string[] postDataParts = postData.Split(new[] {'&'});

            NameValueCollection postCollection = new NameValueCollection();
            foreach (string postPair in postDataParts)
            {
                string[] pairParts = postPair.Split(new[] {'='});
                if (pairParts.Length == 2)
                {
                    string key = pairParts[0];
                    string val = pairParts[1];
                    key = HttpUtility.UrlDecode(key);
                    val = HttpUtility.UrlDecode(val);
                    postCollection.Add(key, val);
                }
            }

            OnDataPosted(postCollection);
        }

        public void HandleRequest(IAsyncResult result)
        {
            HttpListenerContext context;
            lock (listenerLock)
            {
                if (!isRunning)
                    return;

                context = httpListener.EndGetContext(result);
            }
            CreateNewReceieveContext();

            try
            {
                if (context.Request.HttpMethod == "GET")
                {
                    HandleGetRequest(context);
                }
                else if (context.Request.HttpMethod == "POST")
                {
                    HandlePostRequest(context);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            //context.Response.StatusCode = 500;
        }

        protected virtual void OnDataPosted(NameValueCollection formData)
        {
            DataPosted evt = DataPostedEvent;
            if (evt != null)
            {
                evt(formData);
            }
        }
    }
}
