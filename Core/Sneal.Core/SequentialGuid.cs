﻿#region license
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
using System.Runtime.InteropServices;

namespace Sneal.Core
{
    /// <summary>
    /// Guid factory for generating sequential Guids.
    /// </summary>
    public static class SequentialGuid
    {
        private const int Success = 0;

        [DllImport("rpcrt4.dll", SetLastError = true)]
        static extern int UuidCreateSequential(out Guid guid);

        /// <summary>
        /// Creates a new sequential Guid using the Win32 UuidCreateSequential() method.
        /// </summary>
        /// <returns></returns>
        public static Guid NewGuid()
        {
            Guid g;
            return UuidCreateSequential(out g) != Success ? Guid.NewGuid() : g;
        }
    }
}
