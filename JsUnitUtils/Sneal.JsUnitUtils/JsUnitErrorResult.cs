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

namespace Sneal.JsUnitUtils
{
    /// <summary>
    /// Describes the outcome state of a JsUnit test run.
    /// </summary>
    public enum TestResult
    {
        /// <summary>
        /// The test didn't run or return a result.
        /// </summary>
        None,

        /// <summary>
        /// The test ran and passed.
        /// </summary>
        Success,

        /// <summary>
        /// The test did not run or failed because of a JScript error.
        /// </summary>
        Error,

        /// <summary>
        /// The test ran but an assertion failed.
        /// </summary>
        Failure
    }

    /// <summary>
    /// Simple DTO that describes a JsUnit test run.
    /// </summary>
    [Serializable]
    public class JsUnitErrorResult : IFormattable
    {
        public string TestPage { get; set; }
        public string FunctionName { get; set; }
        public string Timing { get; set; }
        public string Message { get; set; }
        public TestResult TestResult { get; set; }

        /// <summary>
        /// Returns <c>true</c> for any outcome other than the test running
        /// and passing.
        /// </summary>
        public bool IsError
        {
            get { return TestResult != TestResult.Success; }
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (formatProvider != null)
            {
                var formatter = formatProvider.GetFormat(GetType()) as ICustomFormatter;
                if (formatter != null)
                {
                    return formatter.Format(format, this, formatProvider);
                }
            }

            return FunctionName + ": " + Message;
        }

        public override string ToString()
        {
            return ToString(null, null);
        }
    }
}
