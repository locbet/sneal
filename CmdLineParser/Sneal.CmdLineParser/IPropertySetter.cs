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

namespace Sneal.CmdLineParser
{
    /// <summary>
    /// Strategy for setting a property on an options instance.
    /// </summary>
    public interface IPropertySetter
    {
        /// <summary>
        /// Sets the property associated with the given PropertyInfoSwitchAttributePair
        /// on the instanceToSet.
        /// </summary>
        /// <param name="propertyPair"></param>
        /// <param name="instanceToSet"></param>
        /// <param name="rawValue"></param>
        void SetPropertyValue(PropertyInfoSwitchAttributePair propertyPair, 
            object instanceToSet, string rawValue);

        /// <summary>
        /// The .NET type that this property setter supports.
        /// </summary>
        Type SupportedType { get; }
    }
}