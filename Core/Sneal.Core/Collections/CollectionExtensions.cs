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

using System.Collections.Generic;

namespace Sneal.Core.Collections
{
    /// <summary>
    /// Extensions methods for ICollection.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Adds all items in the specified enumerable to the ICollection.
        /// </summary>
        /// <typeparam name="T">The type of the item to add</typeparam>
        /// <param name="self">The ICollection instance to add to</param>
        /// <param name="itemsToAdd">The enumerable items to add.</param>
        public static void AddAll<T>(this ICollection<T> self, IEnumerable<T> itemsToAdd)
        {
            foreach (var item in itemsToAdd)
            {
                self.Add(item);
            }
        }
    }
}
