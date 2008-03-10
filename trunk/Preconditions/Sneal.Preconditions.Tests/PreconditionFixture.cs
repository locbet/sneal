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