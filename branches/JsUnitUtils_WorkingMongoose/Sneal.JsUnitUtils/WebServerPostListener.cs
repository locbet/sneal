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

using System.Collections.Specialized;
using System.Threading;
using Sneal.JsUnitUtils.Servers;

namespace Sneal.JsUnitUtils
{
    public class WebServerPostListener : IResultListener
    {
        private const int DefaultTimeout = 4*60*1000;
        private string testCaseResults;

        public WebServerPostListener(IWebServer server)
        {
            server.DataPostedEvent += HandleDataPosted;
        }

        private void HandleDataPosted(NameValueCollection formData)
        {
            if (!string.IsNullOrEmpty(formData["testCases"]))
            {
                testCaseResults = formData["testCases"];
            }
        }

        public string WaitForResults()
        {
            return WaitForResults(DefaultTimeout);
        }

        public string WaitForResults(int timeoutInMilliseconds)
        {
            testCaseResults = null;
            int elapedTime = 0;
            while (testCaseResults == null && elapedTime < timeoutInMilliseconds)
            {
                Thread.Sleep(200);
                elapedTime += 200;
            }
            return testCaseResults;
        }
    }
}
