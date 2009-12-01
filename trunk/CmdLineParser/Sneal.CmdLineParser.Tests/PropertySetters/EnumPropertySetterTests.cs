using System.Reflection;
using NUnit.Framework;
using Sneal.CmdLineParser.PropertySetters;

namespace Sneal.CmdLineParser.Tests.PropertySetters
{
    [TestFixture]
    public class EnumPropertySetterTests
    {
        private EnumPropertyStub _enumStub;
        private EnumPropertySetter _enumSetter;

        [SetUp]
        public void SetUp()
        {
            _enumStub = new EnumPropertyStub();
            _enumSetter = new EnumPropertySetter();
        }

        [Test]
        public void Should_parse_enum_string()
        {
            _enumSetter.SetPropertyValue(
                new Option(),
                _enumStub.GetPropertyInfo(),
                _enumStub,
                "Value1");

            Assert.AreEqual(EnumPropertyStub.MyEnum.Value1, _enumStub.EnumProperty);
        }

        [Test]
        [ExpectedException(typeof(CommandLineException))]
        public void Should_throw_exception_when_bad_enum_value()
        {
            _enumSetter.SetPropertyValue(
                new Option(),
                _enumStub.GetPropertyInfo(),
                _enumStub,
                "Value123");
        }

        [Test]
        [ExpectedException(typeof(CommandLineException))]
        public void Should_throw_exception_when_null_value()
        {
            _enumSetter.SetPropertyValue(
                new Option(),
                _enumStub.GetPropertyInfo(),
                _enumStub,
                null);
        }

        [Test]
        public void SupportsType_should_return_true_when_enum_type()
        {
            Assert.IsTrue(_enumSetter.SupportsType(typeof (EnumPropertyStub.MyEnum)));
        }

        [Test]
        public void SupportsType_should_return_false_when_non_enum_type()
        {
            Assert.IsFalse(_enumSetter.SupportsType(typeof(string)));
            Assert.IsFalse(_enumSetter.SupportsType(typeof(int)));
            Assert.IsFalse(_enumSetter.SupportsType(typeof(bool)));
        }
    }

    public class EnumPropertyStub
    {
        public enum MyEnum
        {
            Value1,
            Value2
        }

        public MyEnum EnumProperty { get; set; }

        public PropertyInfo GetPropertyInfo()
        {
            return typeof(EnumPropertyStub).GetProperties()[0];
        }
    }
}
