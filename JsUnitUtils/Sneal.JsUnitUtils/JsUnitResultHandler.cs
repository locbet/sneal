﻿#region license
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
using System.Diagnostics;
using System.IO.Pipes;
using System.Web;
using System.IO;

namespace Sneal.JsUnitUtils
{
    /// <summary>
    /// Gets the posted JSUnit results and uses named pipes to send raw results
    /// to parent process.
    /// </summary>
    public class JSUnitResultHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            string testCases = context.Request.Params["testCases"];
            if (string.IsNullOrEmpty(testCases))
            {
                testCases = Constants.NoResultsMessage;
            }

            Trace.WriteLine("Raw posted results from ASHX handler:");
            Trace.Write(testCases);

            // named pipes has issues with newlines
            testCases = testCases.Replace("\r", "").Replace("\n", "");

            try
            {
                using (var pipeStream = new NamedPipeClientStream(
                    ".", Constants.JsUnitResultNamedPipe, PipeDirection.Out))
                {
                    pipeStream.Connect(20000);

                    using (var sw = new StreamWriter(pipeStream))
                    {
                        sw.AutoFlush = true;
                        sw.Write(testCases);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }

            HttpContext.Current.Response.StatusCode = 200;
            HttpContext.Current.Response.End();
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}
