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

namespace Sneal.Assertions.Constraints
{
    /// <summary>
    /// Int32 specific exception constraints class.
    /// </summary>
    /// <typeparam name="ExceptionT">The type of exception.</typeparam>
    public class IntExceptionConstraint<ExceptionT> : ExceptionConstraint<ExceptionT, int>
        where ExceptionT : Exception, new()
    {
        public IntExceptionConstraint(int instanceToCheck, string msg)
            : base(instanceToCheck, msg)
        {
        }

        public virtual void IsLessThan(int compare)
        {
            if (instanceToCheck < compare)
                ThrowException();
        }

        public virtual void IsLessThanOrEqual(int compare)
        {
            if (instanceToCheck <= compare)
                ThrowException();
        }

        public virtual void IsGreaterThan(int compare)
        {
            if (instanceToCheck > compare)
                ThrowException();
        }

        public virtual void IsGreaterThanOrEqual(int compare)
        {
            if (instanceToCheck >= compare)
                ThrowException();
        }
    }
}