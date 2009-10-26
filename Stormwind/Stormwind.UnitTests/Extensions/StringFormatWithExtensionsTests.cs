using System;
using Stormwind.Extensions;
using NUnit.Framework;

namespace Stormwind.UnitTests.Extensions
{
    [TestFixture]
    public class StringFormatWithExtensionsTests
    {
        [Test]
        public void FormatWith_using_anonymous_type()
        {
            "bla {FirstName} {LastName}".FormatWith(new { FirstName = "Shawn", LastName = "Neal" })
                .Should().Be.EqualTo("bla Shawn Neal");
        }

        [Test]
        public void FormatWith_using_properties()
        {
            FormatWithTestObject o = new FormatWithTestObject { FirstName = "Shawn", LastName = "Neal" };
             "bla {FirstName} {LastName} dbl bla".FormatWith(o)
                 .Should().Be.EqualTo("bla Shawn Neal dbl bla");
        }
    }

    public class FormatWithTestObject
    {
        public string FirstName { get;set; }
        public string LastName { get;set; }
    }
}
