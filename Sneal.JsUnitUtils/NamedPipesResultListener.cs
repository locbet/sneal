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
using System.IO.Pipes;
using System.Threading;

namespace Sneal.JsUnitUtils
{
    /// <summary>
    /// Retrieves JSUnit results via a named pipe listener.
    /// </summary>
    public class NamedPipesResultListener : IResultListener
    {
        private const int DefaultTimeout = 60000;

        public string WaitForResults()
        {
            return WaitForResults(DefaultTimeout);
        }

        public string WaitForResults(int timeoutInMilliseconds)
        {
            string results = null;

            using (var pipeStream = new NamedPipeServerStream(
                Constants.JsUnitResultNamedPipe,
                PipeDirection.InOut, 2, PipeTransmissionMode.Byte, PipeOptions.Asynchronous))
            {
                IAsyncResult ar = pipeStream.BeginWaitForConnection(null, null);
                if (WaitHandle.WaitAll(new[] { ar.AsyncWaitHandle }, timeoutInMilliseconds, false))
                {
                    pipeStream.EndWaitForConnection(ar);
                    using (var sr = new StreamReader(pipeStream))
                    {
                        results = sr.ReadLine();
                    }
                }
            }

            if (results == Constants.NoResultsMessage)
                results = null;

            return results;
        }
    }
}