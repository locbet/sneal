#region license
// Copyright 2010 Shawn Neal (sneal@sneal.net)
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
using System.Linq.Expressions;

namespace Sneal.Core
{
    /// <summary>
    /// Helper class for guard statements, which allow prettier
    /// code for guard clauses
    /// </summary>
    public class Guard
    {
        /// <summary>
        /// Will throw a <see cref="ArgumentException"/> if the instance is null,
        /// with the specificied message.
        /// </summary>
        /// <param name="expression">An expression which contains the instance to test.</param>
        /// <example>
        /// Sample usage:
        /// <code>
        /// Guard.AgainstNull(() => myinstance));
        /// </code>
        /// </example>
        public static void AgainstNull<T>(
            [AssertionCondition(AssertionConditionType.IS_NOT_NULL)]
            Expression<Func<T>> expression) where T : class
        {
            if (expression.Compile().Invoke() == null)
            {
                throw new ArgumentNullException(
                    string.Format(
                        "The parameter {0} must not be null",
                        GetParamName(expression)));
            }
        }

        /// <summary>
        /// Will throw a <see cref="ArgumentException"/> if the instance is null,
        /// with the specificied message.
        /// </summary>
        /// <param name="instance">The instance to test</param>
        /// <param name="message">The exception message.</param>
        /// <example>
        /// Sample usage:
        /// <code>
        /// Guard.AgainstNull(myinstance, "This param cannot be null");
        /// </code>
        /// </example>
        [AssertionMethod]
        public static void AgainstNull(
            [AssertionCondition(AssertionConditionType.IS_NOT_NULL)]
            object instance,
            string message)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(message);
            }
        }


        /// <summary>
        /// Will throw a <see cref="ArgumentException"/> if the instance is null or empty,
        /// with the specificied message.
        /// </summary>
        /// <param name="expression">An expression which contains the string instance</param>
        /// <example>
        /// Sample usage:
        /// <code>
        /// Guard.AgainstNullOrEmpty(() => mystring));
        /// </code>
        /// </example>
        public static void AgainstNullOrEmpty(
            [AssertionCondition(AssertionConditionType.IS_NOT_NULL)]
    		Expression<Func<string>> expression)
        {
            if (expression.Compile().Invoke().IsNullOrEmpty())
            {
                throw new ArgumentException(
                    string.Format(
                        "The parameter named {0} must not be null or empty",
                        GetParamName(expression)));
            }
        }

        /// <summary>
        /// Will throw a <see cref="ArgumentException"/> if the instance is null or empty,
        /// with the specificied message.
        /// </summary>
        /// <param name="str">The string instance to test</param>
        /// <param name="message">The message.</param>
        /// <example>
        /// Sample usage:
        /// <code>
        /// Guard.AgainstNullOrEmpty(name), "Name must have a value");
        /// </code>
        /// </example>
        [AssertionMethod]
        public static void AgainstNullOrEmpty(
            [AssertionCondition(AssertionConditionType.IS_NOT_NULL)]
            string str,
            string message)
        {
            if (str.IsNullOrEmpty())
            {
                throw new ArgumentException(message);
            }
        }

        /// <summary>
        /// Will throw a <see cref="InvalidOperationException"/> if the assertion
        /// is true, with the specificied message.
        /// </summary>
        /// <param name="assertion">if set to <c>true</c> [assertion].</param>
        /// <param name="message">The message.</param>
        /// <example>
        /// Sample usage:
        /// <code>
        /// Guard.Against(string.IsNullOrEmpty(name), "Name must have a value");
        /// </code>
        /// </example>
        [AssertionMethod]
        public static void Against(
            [AssertionCondition(AssertionConditionType.IS_FALSE)]
			bool assertion,
            string message)
        {
            if (assertion)
            {
                throw new ArgumentException(message);
            }
        }

        /// <summary>
        /// Will throw exception of type <typeparamref name="TException"/>
        /// with the specified message if the assertion is true
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="assertion">if set to <c>true</c> [assertion].</param>
        /// <param name="message">The message.</param>
        /// <example>
        /// Sample usage:
        /// <code>
        /// <![CDATA[
        /// Guard.Against<ArgumentException>(string.IsNullOrEmpty(name), "Name must have a value");
        /// ]]>
        /// </code>
        /// </example>
        [AssertionMethod]
        public static void Against<TException>(
            [AssertionCondition(AssertionConditionType.IS_FALSE)]
			bool assertion,
            string message) where TException : Exception
        {
            if (assertion)
            {
                throw (TException)Activator.CreateInstance(typeof(TException), message);
            }
        }

        private static string GetParamName<T>(Expression<Func<T>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression != null)
            {
                return memberExpression.Member.Name;
            }
            return "";
        }
    }
}
