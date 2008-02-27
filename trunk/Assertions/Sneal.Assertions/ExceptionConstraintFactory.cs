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
    /// Factory class for producing exception constraint instances.
    /// </summary>
    /// <typeparam name="ExceptionT"></typeparam>
    public class ExceptionConstraintFactory<ExceptionT> where ExceptionT : Exception, new()
    {
        private readonly string exceptionMessage;

        public ExceptionConstraintFactory()
        {
        }

        public ExceptionConstraintFactory(string exceptionMessage)
        {
            this.exceptionMessage = exceptionMessage;
        }

        public StringExceptionConstraint<ExceptionT> When(string instanceToCheck)
        {
            return new StringExceptionConstraint<ExceptionT>(instanceToCheck, exceptionMessage);
        }

        public IntExceptionConstraint<ExceptionT> When(int instanceToCheck)
        {
            return new IntExceptionConstraint<ExceptionT>(instanceToCheck, exceptionMessage);
        }

        public ExceptionConstraint<ExceptionT, InstanceT> When<InstanceT>(InstanceT instanceToCheck)
        {
            return new ExceptionConstraint<ExceptionT, InstanceT>(instanceToCheck, exceptionMessage);
        }
    }
}