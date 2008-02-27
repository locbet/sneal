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
    public class ExceptionConstraintFixture
    {
        private object nullInstance;
        private object instance = new object();

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowWhenNull()
        {
            Throw<ArgumentNullException>.When(nullInstance).IsNull();
        }

        [Test]
        public void ShouldThrowWhenNotNull()
        {
            Throw<ArgumentNullException>.When(instance).IsNull();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowWhenEqualToNull()
        {
            Throw<ArgumentNullException>.When(nullInstance).IsEqualTo(null);
        }

        [Test]
        public void ShouldNotThrowWhenEqualObjectAreNotEqual()
        {
            Throw<ArgumentNullException>.When(nullInstance).IsEqualTo(instance);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowWhenNotEqual()
        {
            Throw<ArgumentNullException>.When(nullInstance).IsNotEqualTo(instance);
        }

        [Test]
        public void ShouldNotThrowWhenBothNull()
        {
            Throw<ArgumentNullException>.When(nullInstance).IsNotEqualTo(null);
        }

        [Test]
        public void ShouldNotThrowWhenBothAreSameInstances()
        {
            Throw<ArgumentNullException>.When(instance).IsNotEqualTo(instance);
        }

        [Test]
        public void ShouldThrowSpecifiedMessage()
        {
            try
            {
                Throw<ArgumentNullException>.WithMessage("my customer exception message")
                    .When(nullInstance).IsNull();
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("my customer exception message", ex.Message);
            }
        }
    }
}