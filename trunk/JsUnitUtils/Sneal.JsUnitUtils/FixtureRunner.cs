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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sneal.JsUnitUtils.Browsers;
using Sneal.Preconditions.Aop;

namespace Sneal.JsUnitUtils
{
    /// <summary>
    /// Runs a specific JSUnit test fixture (html file).
    /// </summary>
    public class FixtureRunner
    {
        private readonly IWebBrowser webBrowser;
        private readonly IResultListener resultListener;
        private readonly IResultParser resultParser;
        private IList<JsUnitErrorResult> errors;

        /// <summary>
        /// Contructs a new fixture runner
        /// </summary>
        /// <param name="resultListener">Used to get the JSUnit results</param>
        /// <param name="webBrowser">The browser object used to run the test</param>
        /// <param name="resultParser">Parser used to extract the JSUnit results</param>
        public FixtureRunner(
            [NotNull] IResultListener resultListener,
            [NotNull] IWebBrowser webBrowser,
            [NotNull] IResultParser resultParser)
        {
            this.resultListener = resultListener;
            this.webBrowser = webBrowser;
            this.resultParser = resultParser;
        }

        /// <summary>
        /// Runs the JSUnit test fixture.  This method blocks until the results
        /// are returned from JSUnit or a timeout occurs.
        /// </summary>
        /// <remarks>
        /// Assumes the web server serving the JSUnit fixture page is already running
        /// if one is required.
        /// </remarks>
        /// <param name="jsUnitRunnerUrl">
        /// The full URL of the JSUnit runner html page along with any querystring params
        /// </param>           
        /// <param name="timeoutInMilliseconds">
        /// The maximum amount of time in milliseconds to wait for a result.
        /// </param>    
        /// <returns><c>true</c> if no test errors occurred.</returns>
        public bool RunFixture([NotNull] Uri jsUnitRunnerUrl, int timeoutInMilliseconds)
        {
            // this call is sync, so no results ever get posted...
            webBrowser.OpenUrl(jsUnitRunnerUrl);

            string rawResults = resultListener.WaitForResults(timeoutInMilliseconds);

            webBrowser.Close();

            errors = resultParser.ParseJsUnitErrors(rawResults);
            return errors.Count == 0;            
        }

        /// <summary>
        /// Returns a list of error that occurred during the last run, if any.
        /// </summary>
        public IList<JsUnitErrorResult> Errors
        {
            get { return new ReadOnlyCollection<JsUnitErrorResult>(errors); }
        }
    }
}