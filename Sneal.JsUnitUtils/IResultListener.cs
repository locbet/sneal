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

namespace Sneal.JsUnitUtils
{
    /// <summary>
    /// JsUnit result listener waits for results from the ASHX handler.
    /// </summary>
    public interface IResultListener
    {
        /// <summary>
        /// Waits the default timeout period for results.
        /// </summary>
        /// <returns>The raw results if any.</returns>
        string WaitForResults();

        /// <summary>
        /// Waits the specified timeout period for the results.
        /// </summary>
        /// <param name="timeoutInMilliseconds">
        /// The time in milliseconds to wait for results before timing out.
        /// </param>
        /// <returns>The raw results if any.</returns>
        string WaitForResults(int timeoutInMilliseconds);
    }
}