#region license
// Copyright 2009 Shawn Neal (sneal@sneal.net)
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
using WatiN.Core.Interfaces;

namespace Sneal.TestUtils.Watin
{
    public class LooseStringCompare : ICompare
    {
        private readonly string valueToCompareWith;

        public LooseStringCompare(string valueToCompareWith)
        {
            this.valueToCompareWith = (valueToCompareWith ?? "").Trim();
        }

        public bool Compare(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            string curValue = value.Trim();
            string targetValue = valueToCompareWith;

            if (IsRelativeComparison())
            {
                targetValue = targetValue.Substring(1);
                int startPos = curValue.Length - targetValue.Length + 1;
                if (startPos > 0)
                    curValue = curValue.Substring(startPos);
            }

            return string.Compare(targetValue, curValue,
                StringComparison.InvariantCultureIgnoreCase) == 0;
        }

        private bool IsRelativeComparison()
        {
            return valueToCompareWith.StartsWith("~");
        }
    }
}
