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
using Sneal.Assertions.Constraints;

namespace Sneal.Assertions
{
    /// <summary>
    /// Static factory for creating runtime exceptions via ExceptionConstraints.
    /// </summary>
    /// <remarks>
    /// Example:
    /// <c>Throw&gt;ArgumentNullException&gt;.When(argument).IsNull();</c>
    /// </remarks>
    /// <typeparam name="ExceptionT">The exception type to throw.</typeparam>
    public static class Throw<ExceptionT> where ExceptionT : Exception, new()
    {
        private static readonly ExceptionConstraintFactory<ExceptionT> constraintFactory =
            new ExceptionConstraintFactory<ExceptionT>();

        public static ExceptionConstraint<ExceptionT, InstanceT> When<InstanceT>(InstanceT instanceToCheck)
        {
            return constraintFactory.When(instanceToCheck);
        }

        public static StringExceptionConstraint<ExceptionT> When(string instanceToCheck)
        {
            return constraintFactory.When(instanceToCheck);
        }

        public static IntExceptionConstraint<ExceptionT> When(int instanceToCheck)
        {
            return constraintFactory.When(instanceToCheck);
        }

        public static ExceptionConstraintFactory<ExceptionT> WithoutMessage()
        {
            return new ExceptionConstraintFactory<ExceptionT>();
        }

        public static ExceptionConstraintFactory<ExceptionT> WithMessage(string msg)
        {
            return new ExceptionConstraintFactory<ExceptionT>(msg);
        }
    }
}