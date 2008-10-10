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

namespace Sneal.Preconditions.Tests
{
    [TestFixture]
    public class StringPreconditionFixture
    {
        [Test]
        [ExpectedException(typeof (ArgumentException))]
        public void ShouldAssertArgumentDoesNotMatch()
        {
            Throw.If("1").DoesNotMatch("2");
        }

        [Test]
        [ExpectedException(typeof (ArgumentException))]
        public void ShouldAssertArgumentIsEmpty()
        {
            Throw.If("").IsNullOrEmpty();
        }

        [Test]
        public void ShouldAssertArgumentIsNotEmpty()
        {
            Throw.If("1").IsNullOrEmpty();
        }

        [Test]
        [ExpectedException(typeof (ArgumentException))]
        public void ShouldAssertArgumentLengthIsEqualTo2()
        {
            Throw.If("12").LengthIsLessThanOrEqualTo(2);
        }

        [Test]
        [ExpectedException(typeof (ArgumentException))]
        public void ShouldAssertArgumentLengthIsEqualTo3()
        {
            Throw.If("124").LengthIsGreaterThanOrEqualTo(3);
        }

        [Test]
        [ExpectedException(typeof (ArgumentException))]
        public void ShouldAssertArgumentLengthIsGreaterThan2()
        {
            Throw.If("124").LengthIsGreaterThan(2);
        }

        [Test]
        [ExpectedException(typeof (ArgumentException))]
        public void ShouldAssertArgumentLengthIsLessThan100()
        {
            Throw.If("1").LengthIsLessThan(100);
        }

        [Test]
        public void ShouldAssertArgumentMatches()
        {
            Throw.If("1").DoesNotMatch("1");
        }
    }
}