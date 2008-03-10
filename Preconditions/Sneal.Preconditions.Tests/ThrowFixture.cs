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
using NUnit.Framework.SyntaxHelpers;

namespace Sneal.Preconditions.Tests
{
    [TestFixture]
    public class ThrowFixture
    {
        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "The argument did not meet the specified precondition")]
        public void ShouldThrowWhenPredicateIsTrue()
        {
            object instance = null;
            Throw.If(instance == null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "The argument arg1 did not meet the specified precondition")]
        public void ShouldThrowArgNameWhenPredicateIsTrue()
        {
            object instance = null;
            Throw.If(instance == null, "arg1");
        }

        [Test]
        public void ShouldCreateNewPreCondition()
        {
            Assert.That(Throw.If(1), Is.InstanceOfType(typeof (Precondition<int>)));
        }

        [Test]
        public void ShouldCreateNewPreConditionWithArgName()
        {
            Assert.That(Throw.If(1, "arg1"), Is.InstanceOfType(typeof(Precondition<int>)));
        }

        [Test]
        public void ShouldCreateNewStringPreCondition()
        {
            Assert.That(Throw.If("sneal"), Is.InstanceOfType(typeof(StringPrecondition)));
        }

        [Test]
        public void ShouldCreateNewStringPreConditionWithArgName()
        {
            Assert.That(Throw.If("sneal", "arg1"), Is.InstanceOfType(typeof(StringPrecondition)));
        }
    }
}