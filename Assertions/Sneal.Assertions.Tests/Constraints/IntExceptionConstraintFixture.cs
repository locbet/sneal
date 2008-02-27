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
using NUnit.Framework;

namespace Sneal.Assertions.Tests.Constraints
{
    [TestFixture]
    public class IntExceptionConstraintFixture
    {
        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ShouldThrowWhenLessThan()
        {
            Throw<ArgumentOutOfRangeException>.When(5).IsLessThan(6);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ShouldThrowWhenLessThanOrEqual()
        {
            Throw<ArgumentOutOfRangeException>.When(8).IsLessThanOrEqual(8);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ShouldThrowWhenGreaterThan()
        {
            Throw<ArgumentOutOfRangeException>.When(8).IsGreaterThan(6);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ShouldThrowWhenGreaterThanOrEqual()
        {
            Throw<ArgumentOutOfRangeException>.When(8).IsGreaterThanOrEqual(8);
        }
    }
}