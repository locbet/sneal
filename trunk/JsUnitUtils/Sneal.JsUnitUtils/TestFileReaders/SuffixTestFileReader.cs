﻿#region license
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

using Sneal.Preconditions.Aop;

namespace Sneal.JsUnitUtils.TestFileReaders
{
    public class SuffixTestFileReader : AbstractTestFileReader
    {
        private readonly string postFix;

        public SuffixTestFileReader([NotNullOrEmpty] string postFix, [NotNull] ITestFileReader adaptee)
            : base(adaptee)
        {
            this.postFix = postFix;
        }

        protected override bool CurrentFileShouldPassThrough([NotNull] string currentFile)
        {
            return currentFile.EndsWith(postFix);
        }
    }
}