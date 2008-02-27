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
    public class StringExceptionConstraintFixture
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowWhenEmpty()
        {
            Throw<ArgumentNullException>.When("").IsNullOrEmpty();
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ShouldThrowLessThanLength()
        {
            Throw<ArgumentOutOfRangeException>.When("").LengthIsLessThan(1);
        }

        [Test]
        public void ShouldNotThrowWhenLengthIsGreaterThan()
        {
            Throw<ArgumentOutOfRangeException>.When("hello").LengthIsLessThan(1);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ShouldThrowGreaterThanLength()
        {
            Throw<ArgumentOutOfRangeException>.When("hello").LengthIsGreaterThan(1);
        }

        [Test]
        public void ShouldNotThrowWhenLengthIsLessThan()
        {
            Throw<ArgumentOutOfRangeException>.When("hello").LengthIsGreaterThan(10);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ShouldThrowWhenPatternDoesNotMatch()
        {
            Throw<ArgumentOutOfRangeException>.When("hello").DoesNotMatchPattern("bye");
        }

        [Test]
        public void ShouldNotThrowWhenPatternDoesMatch()
        {
            Throw<ArgumentOutOfRangeException>.When("hello").DoesNotMatchPattern("hello");
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ShouldThrowWhenPatternDoesMatch()
        {
            Throw<ArgumentOutOfRangeException>.When("hello").MatchesPattern("hello");
        }
    }
}