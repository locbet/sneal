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
            Throw.If("").IsEmpty();
        }

        [Test]
        public void ShouldAssertArgumentIsNotEmpty()
        {
            Throw.If("1").IsEmpty();
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