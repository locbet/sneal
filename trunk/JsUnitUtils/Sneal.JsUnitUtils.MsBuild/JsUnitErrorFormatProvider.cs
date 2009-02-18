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

namespace Sneal.JsUnitUtils.MsBuild
{
    /// <summary>
    /// Formats a <see cref="JsUnitErrorResult"/> for use in MSBuild output.
    /// </summary>
    public class JsUnitErrorFormatProvider : IFormatProvider, ICustomFormatter
    {
        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            var result = arg as JsUnitErrorResult;
            if (result == null)
            {
                return arg.ToString();
            }

            if (result.TestResult == TestResult.Error)
            {
                return string.Format(
                    "{0}: {1} had a JavaScript error occur during execution",
                    result.TestPage, result.FunctionName);
            }
            if (result.TestResult == TestResult.Failure)
            {
                return string.Format(
                    "{0}: {1} failed with message: {2}",
                    result.TestPage, result.FunctionName, result.Message); 
            }
            if (result.TestResult == TestResult.Success)
            {
                return string.Format(
                    "{0}: {1} passed",
                    result.TestPage, result.FunctionName);
            }
            
            return string.Format(
                "{0}: {1} Didn't run or failed for an unknown reason",
                result.TestPage, result.FunctionName);
        }

        public object GetFormat(Type formatType)
        {
            return this;
        }
    }
}
