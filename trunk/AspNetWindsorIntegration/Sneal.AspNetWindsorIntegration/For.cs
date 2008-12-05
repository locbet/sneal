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

namespace Sneal.AspNetWindsorIntegration
{
    /// <summary>
    /// Optional behavior settings for the page PagsUsesInjection attribute.
    /// </summary>
    public enum For
    {
        /// <summary>
        /// All public writable properties will be treated as if they were
        /// maked with the OptionalDependencyAttribute regardless, unless they
        /// are marked with the RequiredDependencyAttribute.  This is the
        /// default behavior.
        /// </summary>
        AllProperties,

        /// <summary>
        /// All public properties with a RequiredDependencyAttribute or a
        /// OptionalDependencyAttribute will be considered for injection,
        /// all other properties are ignored.
        /// </summary>
        ExplicitProperties,

        /// <summary>
        /// No properties will be injected.
        /// </summary>
        None
    }
}