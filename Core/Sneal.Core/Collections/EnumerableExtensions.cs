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
using System.Collections.Generic;

namespace Sneal.Core.Collections
{
    /// <summary>
    /// IEnumerable extension methods.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Iterates through each item and calls the specified action.
        /// </summary>
        /// <typeparam name="T">The type of the object to enumerate</typeparam>
        /// <param name="enumerable">The enumerable collection</param>
        /// <param name="action">The action to apply to each item in the collection</param>
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var instance in enumerable)
            {
                action(instance);
            }
        }
    }
}
