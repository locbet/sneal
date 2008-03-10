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
    public class PreconditionFixture
    {
        [Test]
        public void ShouldAssertArgumentAreComparable()
        {
            Throw.If(1).IsComparableTo(3);
        }

        [Test]
        [ExpectedException(typeof (ArgumentException))]
        public void ShouldAssertArgumentAreEqual()
        {
            Throw.If(1).IsEqualTo(1);
        }

        [Test]
        public void ShouldAssertArgumentAreNotEqual()
        {
            Throw.If(1).IsEqualTo(2);
        }

        [Test]
        [ExpectedException(typeof (ArgumentException))]
        public void ShouldAssertArgumentIsGreaterThan()
        {
            Throw.If(1).IsGreaterThan(0);
        }

        [Test]
        [ExpectedException(typeof (ArgumentException))]
        public void ShouldAssertArgumentIsLessThan()
        {
            Throw.If(0).IsLessThan(1);
        }

        [Test]
        public void ShouldAssertArgumentIsNotNull()
        {
            Throw.If(new object()).IsNull();
        }

        [Test]
        [ExpectedException(typeof (ArgumentNullException))]
        public void ShouldAssertArgumentIsNull()
        {
            Throw.If(null).IsNull();
        }
    }
}