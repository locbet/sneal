#region license
// Copyright 2008 Shawn Neal (neal.shawn@gmail.com)
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
using System.Text.RegularExpressions;

namespace Sneal.Assertions.Constraints
{
    /// <summary>
    /// String specific exception constraints.
    /// </summary>
    /// <typeparam name="ExceptionT">The type of exception.</typeparam>
    public class StringExceptionConstraint<ExceptionT> : ExceptionConstraint<ExceptionT, string> where ExceptionT : Exception, new()
    {
        public StringExceptionConstraint(string instanceToCheck, string msg)
            : base(instanceToCheck, msg) { }

        public virtual void IsNullOrEmpty()
        {
            if (string.IsNullOrEmpty(instanceToCheck))
                ThrowException();
        }

        public virtual void LengthIsLessThan(int len)
        {
            if (instanceToCheck == null || instanceToCheck.Length < len)
                ThrowException();
        }

        public virtual void LengthIsGreaterThan(int len)
        {
            if (instanceToCheck == null || instanceToCheck.Length > len)
                ThrowException();
        }

        public virtual void DoesNotMatchPattern(string regexPattern)
        {
            Regex regex = new Regex(regexPattern);
            if (!regex.Match(instanceToCheck).Success)
                ThrowException();
        }

        public virtual void MatchesPattern(string regexPattern)
        {
            Regex regex = new Regex(regexPattern);
            if (regex.Match(instanceToCheck).Success)
                ThrowException();
        }
    }
}