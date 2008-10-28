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
using Sneal.JsUnitUtils.Browsers;
using Sneal.Preconditions.Aop;

namespace Sneal.JsUnitUtils
{
    /// <summary>
    /// Wraps an implementation of a class that will open and close a web browser.
    /// </summary>
    public interface IWebBrowser : IDisposable
    {
        /// <summary>
        /// Opens the web browser to the specified URL and loads the page. This
        /// is a non-blocking operation, i.e. this method will return before the
        /// browser finishes loading.
        /// </summary>
        /// <param name="url">The url to open.</param>
        void OpenUrl([NotNull] Uri url);

        /// <summary>
        /// Closes the browser window regardless of state.
        /// </summary>
        void Close();

        /// <summary>
        /// The browser implementation type: IE, Firefox etc.
        /// </summary>
        With BrowserType { get; }
    }
}