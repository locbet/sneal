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
using System.Reflection;

namespace Sneal.Assertions.Constraints
{
    /// <summary>
    /// This is the base exception constraint class which all constraints
    /// should use as a base.
    /// </summary>
    /// <typeparam name="ExceptionT">The exception type.</typeparam>
    /// <typeparam name="InstanceT">The instance type we're checking.</typeparam>
    public class ExceptionConstraint<ExceptionT, InstanceT> where ExceptionT : Exception, new()
    {
        protected readonly InstanceT instanceToCheck;
        protected readonly string msg;

        public ExceptionConstraint(InstanceT instanceToCheck, string msg)
        {
            this.instanceToCheck = instanceToCheck;
            this.msg = msg;
        }

        public virtual void IsNull()
        {
            if (instanceToCheck == null)
                ThrowException();
        }

        public virtual void IsEqualTo(InstanceT compareInstance)
        {
            if (null == compareInstance && null == instanceToCheck)
                ThrowException();

            if (instanceToCheck != null && instanceToCheck.Equals(compareInstance))
                ThrowException();

            if (compareInstance != null && compareInstance.Equals(instanceToCheck))
                ThrowException();
        }

        public virtual void IsNotEqualTo(InstanceT compareInstance)
        {
            if (instanceToCheck != null && !instanceToCheck.Equals(compareInstance))
                ThrowException();

            if (compareInstance != null && !compareInstance.Equals(instanceToCheck))
                ThrowException();
        }

        protected virtual void ThrowException()
        {
            ExceptionT ex = new ExceptionT();

            if (!string.IsNullOrEmpty(msg))
            {
                Type type = ex.GetType();
                FieldInfo messageField = type.GetField("_message", BindingFlags.Instance | BindingFlags.NonPublic);
                if (messageField != null)
                    messageField.SetValue(ex, msg);
            }

            throw ex;
        }
    }
}